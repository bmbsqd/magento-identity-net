using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNet.Identity;

namespace Bmbsqd.Magento.Identity
{
	public class MagentoUser : IUser<string>
	{
		private readonly IMagentoCustomer _customer;
		private readonly ISet<string> _roles;

		public MagentoUser( IMagentoCustomer customer, IEnumerable<string> roles )
		{
			_customer = customer;
			_roles = new HashSet<string>( roles );
		}

		public IMagentoCustomer Customer
		{
			get { return _customer; }
		}

		public string Id
		{
			get { return _customer.CustomerId.ToString( CultureInfo.InvariantCulture ); }
		}

		public string Suffix
		{
			get { return _customer.Suffix; }
		}

		public string Prefix
		{
			get { return _customer.Prefix; }
		}

		public string Lastname
		{
			get { return _customer.Lastname; }
		}

		public string Middlename
		{
			get { return _customer.Middlename; }
		}

		public string Firstname
		{
			get { return _customer.Firstname; }
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
			get { return _roles.ToArray(); }
		}

		public int CustomerId
		{
			get { return _customer.CustomerId; }
		}
	}
}