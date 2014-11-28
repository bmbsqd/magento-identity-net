using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bmbsqd.Magento.WebService;
using Microsoft.AspNet.Identity;

namespace Bmbsqd.Magento.Identity
{
	public abstract class MagentoUserStoreBase<TUser> :
		IUserStore<TUser>,
		IUserEmailStore<TUser>,
		IPasswordHasher,
		IUserPasswordStore<TUser>,
		IUserRoleStore<TUser>
		where TUser : MagentoUser
	{
		private readonly IMagentoSessionFactory _sessionFactory;
		private Task<IDictionary<int, string>> _groupMapTask;
		private Task<IMagentoSession> _sessionTask;

		protected MagentoUserStoreBase( IMagentoSessionFactory sessionFactory )
		{
			_sessionFactory = sessionFactory;
		}

		private Task<IMagentoSession> LoginAsync()
		{
			return _sessionFactory.CreateSessionAsync();
		}

		protected Task<IMagentoSession> Session
		{
			get { return _sessionTask ?? (_sessionTask = LoginAsync()); }
		}

		protected virtual Task<IDictionary<int, string>> AllGroups
		{
			get { return _groupMapTask ?? (_groupMapTask = Session.Then( session => session.ListCustomerGroupsAsync() )); }
		}

		protected abstract Task<TUser> CreateUser( IMagentoCustomer customer );

		public virtual void Dispose()
		{
			Session
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
				var customer = await Session.Then( session => session.GetCustomerByIdAsync( customerId ) );
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

		public virtual string HashPassword( string password )
		{
			return password;
		}

		public virtual PasswordVerificationResult VerifyHashedPassword( string hashedPassword, string providedPassword )
		{
			return MagentoPasswordValidator.ValidatePassword( hashedPassword, providedPassword )
				? PasswordVerificationResult.Success
				: PasswordVerificationResult.Failed;
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
			return Session.Then( async session => {
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
			var customers = await Session.Then( session => session.FindCustomersAsync( new CustomerFilter { Email = email } ) );
			if( customers.Count == 1 ) {
				return await CreateUser( customers[0] );
			}

			if( customers.Count > 1 ) {
				Trace.WriteLine( "Multiple customers matched " + email );
			}
			return null;
		}
	}
}
