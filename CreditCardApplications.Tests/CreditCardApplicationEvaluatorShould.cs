using Xunit;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        [Fact]
        public void AcceptingHighIncomeApplecations()
        {
            //sut = systemUnderTest
            //we supply null instead new FrequentFlyerNumberValidatorService()
            var sut = new CreditCardApplicationEvaluator(null);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            //we supply null instead new FrequentFlyerNumberValidatorService()
            var sut = new CreditCardApplicationEvaluator(null);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }
    }
}