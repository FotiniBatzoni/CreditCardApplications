
namespace CreditCardApplications
{
    internal class FraudLookup
    {
        public bool isFraudRisk(CreditCardApplication application)
        {
            if( application.LastName == "Smith")
            {
                return true;
            }
            return false;
        }
    }
}
