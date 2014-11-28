using System;
using System.Globalization;
using Microsoft.AspNet.Identity;

namespace Bmbsqd.Magento.Identity
{
	public class MagentoUser : IUser<string>
	{
		private readonly IMagentoCustomer _customer;
		private readonly string[] _roles;

		public MagentoUser( IMagentoCustomer customer, string[] roles )
		{
			_customer = customer;
			_roles = roles;
		}

		public string Id
		{
			get { return _customer.CustomerId.ToString( CultureInfo.InvariantCulture ); }
		}

		public string UserName
		{
			get { return _customer.Email; }
			set { throw new InvalidOperationException(); }
		}

		public string Email
		{
			get { return _customer.Email; }
		}

		public string PasswordHash
		{
			get { return _customer.PasswordHash; }
		}

		public string[] Roles
		{
			get { return _roles; }
		}

		public int CustomerId
		{
			get { return _customer.CustomerId; }
		}
	}
}