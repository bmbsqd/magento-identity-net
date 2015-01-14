using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bmbsqd.Caching;
using Bmbsqd.Magento.WebService;
using Microsoft.AspNet.Identity;

namespace Bmbsqd.Magento.Identity
{
	public abstract class MagentoUserStoreBase
	{
		protected static readonly Task<IList<Claim>> _emptyClaimsTask = Task.FromResult<IList<Claim>>( new Claim[0] );
	}

	public abstract class MagentoUserStoreBase<TUser> : MagentoUserStoreBase,
		IUserStore<TUser>,
		IUserEmailStore<TUser>,
		IUserPasswordStore<TUser>,
		IUserRoleStore<TUser>,
		IUserClaimStore<TUser>
		where TUser : MagentoUser
	{
		protected readonly IMagentoSessionFactory _sessionFactory;
		private readonly ISingleAsyncCache<IDictionary<int, string>> _groupsCache;

		protected MagentoUserStoreBase( IMagentoSessionFactory sessionFactory )
		{
			_sessionFactory = sessionFactory;
			_groupsCache = new SingleAsyncCache<IDictionary<int, string>>( TimeSpan.FromMinutes( 20 ) );
		}

		protected virtual Task<IDictionary<int, string>> AllGroups
		{
			get
			{
				return _groupsCache.GetOrAddAsync( () => _sessionFactory.CreateSessionAsync().Then( session => session.ListCustomerGroupsAsync() ) );
			}
		}

		protected abstract Task<TUser> CreateUser( IMagentoCustomer customer );

		public virtual void Dispose()
		{
			_sessionFactory.CreateSessionAsync()
				.Then( session => session.DisposeAsync() )
				.Wait();
		}

		public virtual Task CreateAsync( TUser user )
		{
			throw new NotImplementedException();
		}

		public virtual Task UpdateAsync( TUser user )
		{
			throw new NotImplementedException();
		}

		public virtual Task DeleteAsync( TUser user )
		{
			throw new NotImplementedException();
		}

		public virtual async Task<TUser> FindByIdAsync( string userId )
		{
			int customerId;
			if( int.TryParse( userId, out customerId ) ) {
				var customer = await _sessionFactory.CreateSessionAsync().Then( session => session.GetCustomerByIdAsync( customerId ) );
				if( customer != null ) {
					return await CreateUser( customer );
				}
			}
			return null;
		}

		public virtual Task<TUser> FindByNameAsync( string userName )
		{
			return FindByEmailAsync( userName );
		}

		public virtual Task SetPasswordHashAsync( TUser user, string passwordHash )
		{
			throw new NotImplementedException();
		}

		public virtual Task<string> GetPasswordHashAsync( TUser user )
		{
			return Task.FromResult( user.PasswordHash );
		}

		public virtual Task<bool> HasPasswordAsync( TUser user )
		{
			return Task.FromResult( !string.IsNullOrEmpty( user.PasswordHash ) );
		}

		public virtual Task AddToRoleAsync( TUser user, string roleName )
		{
			throw new NotImplementedException();
		}

		public virtual Task RemoveFromRoleAsync( TUser user, string roleName )
		{
			throw new NotImplementedException();
		}

		public Task<IList<string>> GetRolesAsync( TUser user )
		{
			return Task.FromResult<IList<string>>( user.Roles );
		}

		public virtual Task<bool> IsInRoleAsync( TUser user, string roleName )
		{
			return Task.FromResult( user.Roles.Contains( roleName ) );
		}

		public virtual Task SetEmailAsync( TUser user, string email )
		{
			// TODO: user object email address must be changed
			return _sessionFactory.CreateSessionAsync().Then( async session => {
				var success = await session.UpdateCustomerAsync( user.CustomerId, new CustomerEntityToCreate {
					Email = email
				} );
				if( !success ) {
					throw new Exception( "Unable to update user email address" );
				}
			} );
		}

		public virtual Task<string> GetEmailAsync( TUser user )
		{
			return Task.FromResult( user.Email );
		}

		public virtual Task<bool> GetEmailConfirmedAsync( TUser user )
		{
			throw new NotImplementedException();
		}

		public virtual Task SetEmailConfirmedAsync( TUser user, bool confirmed )
		{
			throw new NotImplementedException();
		}

		public virtual async Task<TUser> FindByEmailAsync( string email )
		{
			var customers = await _sessionFactory.CreateSessionAsync().Then( session => session.FindCustomersAsync( new CustomerFilter { Email = email } ) );
			if( customers.Count == 1 ) {
				return await CreateUser( customers[0] );
			}

			if( customers.Count > 1 ) {
				Trace.WriteLine( "Multiple customers matched " + email );
			}
			return null;
		}

		public virtual Task<IList<Claim>> GetClaimsAsync( TUser user )
		{
			return _emptyClaimsTask;
		}

		public Task AddClaimAsync( TUser user, Claim claim )
		{
			throw new NotImplementedException();
		}

		public Task RemoveClaimAsync( TUser user, Claim claim )
		{
			throw new NotImplementedException();
		}
	}
}
