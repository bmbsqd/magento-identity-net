using Microsoft.AspNet.Identity;

namespace Bmbsqd.Magento.Identity
{
	public class MagentoPasswordHasher : IPasswordHasher
	{
		public string HashPassword( string password )
		{
			return password;
		}

		public PasswordVerificationResult VerifyHashedPassword( string hashedPassword, string providedPassword )
		{
			return MagentoPasswordValidator.ValidatePassword( hashedPassword, providedPassword )
				? PasswordVerificationResult.Success
				: PasswordVerificationResult.Failed;
		}
	}
}