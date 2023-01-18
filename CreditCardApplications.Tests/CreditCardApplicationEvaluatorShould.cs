using Xunit;
using Moq;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        [Fact]
        public void AcceptingHighIncomeApplecations()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //sut = systemUnderTest
            //we supply null instead new FrequentFlyerNumberValidatorService()
            //var sut = new CreditCardApplicationEvaluator(null);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            //we supply null instead new FrequentFlyerNumberValidatorService()
            //var sut = new CreditCardApplicationEvaluator(null);

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void DeclineLowIncomeApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();


            //1 way
            //to setup isValid to true
            //it returns true only when    FrequentFlyerNumber = "x"

            //1. mockValidator.Setup(x => x.IsValid("x")).Returns(true);

            //2 way
            //it returns true when   FrequentFlyerNumber = is any string

            //2. mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            //3 way
            //it returns true when   FrequentFlyerNumber = is a number which starts with "y"

            //3. mockValidator.Setup(x => x.IsValid(It.Is<string>(number => number.StartsWith("y")))).Returns(true);

            //4 way
            //it returns true when   FrequentFlyerNumber = is in Range from "a" to "z"
            //Moq.Range.Inclusive means that "a" is included to the Range
            //if Moq.Range.Exclusive means that "a" is not included to the Range

            //4. mockValidator.Setup(x => x.IsValid(It.IsInRange<string>("a","z", Range.Inclusive))).Returns(true);

            //5 way
            //it returns true when   FrequentFlyerNumber = is "z" || "y" || "x"

            //5. mockValidator.Setup(x => x.IsValid(It.IsIn("z", "y", "x"))).Returns(true);

            //6 way 
            //it returns true when   FrequentFlyerNumber = match Regex

            mockValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]"))).Returns(true);


            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "y"
            };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Loose);

            //it is neseccery when we use MockBehavior.Strict
         
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var aplication = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(aplication);

           Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);   
        }

        [Fact]
        public void DeclineLowIncomeApplicationsOutDemo()
        {
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            bool isValid = true;
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication
            {
                GrossAnnualIncome = 19_999,
                Age = 42
            };

            CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        }

        [Fact]
        public void ReferWhenLicenceKeyExpired()
        {
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            //Mocking LicenceKey Property
            mockValidator.Setup(x => x.LicenceKey).Returns("EXPIRED");

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication {  Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }
    }
}