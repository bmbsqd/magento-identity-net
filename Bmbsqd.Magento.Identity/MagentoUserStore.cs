using System.Threading.Tasks;

namespace Bmbsqd.Magento.Identity
{
	public class MagentoUserStore : MagentoUserStoreBase<MagentoUser>
	{
		public MagentoUserStore( IMagentoSessionFactory sessionFactory )
			: base( sessionFactory )
		{
		}

		protected override async Task<MagentoUser> CreateUser( IMagentoCustomer customer )
		{
			var allGroups = await AllGroups;
			string group;
			var groups = allGroups.TryGetValue( customer.GroupId, out group ) 
				? new[] { group } 
				: new string[0];
			return new MagentoUser( customer, groups );
		}
	}
}