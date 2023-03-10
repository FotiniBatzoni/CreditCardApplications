using Xunit;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould_Refactored
    {
        private Mock<IFrequentFlyerNumberValidator> mockValidator;
        private CreditCardApplicationEvaluator sut;

        //constructor
        public CreditCardApplicationEvaluatorShould_Refactored()
        {
            mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.SetupAllProperties();
            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns("OK");
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

            sut = new CreditCardApplicationEvaluator(mockValidator.Object);
        }
        [Fact]
        public void AcceptingHighIncomeApplecations_Refactored()
        {

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, decision);
        }

        [Fact]
        public void ReferYoungApplications_Refactored()
        {
            mockValidator.DefaultValue = DefaultValue.Mock;

            var application = new CreditCardApplication { Age = 19 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void DeclineLowIncomeApplications_Refactored()
        {
     
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
        public void ReferInvalidFrequentFlyerApplications_Refactored()
        {

            //it is neseccery when we use MockBehavior.Strict

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var aplication = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(aplication);

           Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);   
        }

        //[Fact]
        //public void DeclineLowIncomeApplicationsOutDemo()
        //{
        //    Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

        //    bool isValid = true;
        //    mockValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid));

        //    var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

        //    var application = new CreditCardApplication
        //    {
        //        GrossAnnualIncome = 19_999,
        //        Age = 42
        //    };

        //    CreditCardApplicationDecision decision = sut.EvaluateUsingOut(application);

        //    Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);
        //}

        //[Fact]
        //public void ReferWhenLicenceKeyExpired()
        //{
        //    var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

        //    mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);

        //    //Mocking LicenceKey Property
        //    //1 way
        //    //mockValidator.Setup(x => x.LicenceKey).Returns("EXPIRED");

        //    //2 way
        //    //when there is a LicenceKey property
        //    //mockValidator.Setup(x => x.LicenceKey).Returns(GetLicenseKeyExpireString);


        //    var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

        //    var application = new CreditCardApplication {  Age = 42 };

        //    CreditCardApplicationDecision decision = sut.Evaluate(application);

        //    Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        //}

        [Fact]
        public void ReferWhenLicenceKeyExpired_Refactored()
        {
            //3 way
            //when we use interfaces
            //var mockLisenceData = new Mock<ILicenceData>();
            //mockLisenceData.Setup(x => x.LicenceKey).Returns("EXPIRED");

            //var mockServiceInfo = new Mock<IServiceInformation>();
            //mockServiceInfo.Setup(x => x.Licence).Returns(mockLisenceData.Object);

            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInfo.Object);

            //4 way
            var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns("EXPIRED");

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);


            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        string GetLicenseKeyExpireString_Refactored()
        {
            //E.g read from vendor-supplied constants file
            return "EXPIRED";
        }

        [Fact]
        public void UseDetailedLookUpForOlderApplication()
        {

            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);

            Assert.Equal(ValidationMode.Deatailed, mockValidator.Object.ValidationMode);
        }

        //[Fact]
        //public void ValidateFrequentFlyerNumberForLowIncomeApplications()
        //{
        //    var mockValidator = new Mock<IFrequentFlyerNumberValidator>();

        //    mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns("OK");

        //    var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

        //    var application = new CreditCardApplication
        //    {
        //        FrequentFlyerNumber = "q"
        //    };


        //    sut.Evaluate(application);

        //    //"Frequent Flyer Number should be validated" is a Custom Error Message
        //    mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), "Frequent Flyer Number should be validated" );
        //}

        [Fact]
        public void ValidateFrequentFlyerNumberForLowIncomeApplications_Refactored()
        {

            var application = new CreditCardApplication
            {
                FrequentFlyerNumber = "q"
            };


            sut.Evaluate(application);

         
            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Once );
        }

        [Fact]
        public void NotValidateFrequentFlyerNumberForHighIncomeApplications_Refactored()
        {

            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            sut.Evaluate(application);

            mockValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void CheckLicenceKeyForLowIncomeApplication_Refactored()
        {

            var application = new CreditCardApplication { GrossAnnualIncome = 99_000 };


            sut.Evaluate(application);

            //Veryfing a property getter was called ( the property has been accessed)
            mockValidator.VerifyGet(x => x.ServiceInformation.Licence.LicenceKey);
        }

        [Fact]
        public void SetDetailedLookUpForOlderApplications_Refactored()
        {
            var application = new CreditCardApplication { Age = 30 };


            sut.Evaluate(application);

            mockValidator.VerifySet(x => x.ValidationMode = It.IsAny<ValidationMode>(), Times.Once);

            //mockValidator.Verify(x => x.IsValid(null), Times.Once);

            //VerifyNoOtherCalls() means that there should be NO other calls in Moq Object 
            //mockValidator.VerifyNoOtherCalls();
        }

        [Fact]
        public void ReferWhenFrequentFlyerValidationError_Refactored()
        {

            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Throws<Exception>();
            //for custom error messages
            //mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Throws(new Exception("Custom message"));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void IncrementLookUpCount_Refactored()
        {

            //use this if you want to raise the event manually
            ///mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Throws<Exception>();
            ///
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>()))
                .Returns(true)
                .Raises(x => x.ValidatorLookupPerformed += null, EventArgs.Empty);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 42 };

            sut.Evaluate(application);

            //raise an event manually
            //mockValidator.Raise(x => x.ValidatorLookupPerformed += null, EventArgs.Empty);

            Assert.Equal(1, sut.ValidatorLookupCount);

        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications_ReturnValuesSequence_Refactored()
        {

           mockValidator.SetupSequence(x => x.IsValid(It.IsAny<string>()))
                .Returns(false)
                .Returns(true);
           
  

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application = new CreditCardApplication { Age = 25 };

            CreditCardApplicationDecision firstDecision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, firstDecision);

            CreditCardApplicationDecision secondDecision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, secondDecision);

        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications_MultipleCallsSequence_Refactored()
        {

            var frequentFlyerNumberPassed = new List<string>();
            mockValidator.Setup(x => x.IsValid(Capture.In(frequentFlyerNumberPassed)));

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

            var application1 = new CreditCardApplication { Age = 25 , FrequentFlyerNumber= "aa" };
            var application2 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "bb" };
            var application3 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "cc" };

            sut.Evaluate(application1);
            sut.Evaluate(application2);
            sut.Evaluate(application3);

            //Assert that isValid was called 3 times with 'aa', 'bb', 'cc
            Assert.Equal(new List<string> { "aa", "bb", "cc" }, frequentFlyerNumberPassed);
        }

        //[Fact]
        //public void ReferFraudRisk()
        //{
        //    Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();  

        //    Mock<FraudLookup> mockFraudLookup = new Mock<FraudLookup>();

        //    mockFraudLookup.Setup(x => x.IsFraudRisk(It.IsAny<CreditCardApplication>())).Returns(true);

        //    var sut = new CreditCardApplicationEvaluator(mockValidator.Object, mockFraudLookup.Object);

        //    var application = new CreditCardApplication();

        //    CreditCardApplicationDecision decision = sut.Evaluate(application);

        //    Assert.Equal(CreditCardApplicationDecision.RefferedToHumanFraudRisk, decision);
        //}

        [Fact]
        public void ReferFraudRisk_Refactored()
        {
            
            Mock<FraudLookup> mockFraudLookup = new Mock<FraudLookup>();

            mockFraudLookup.Protected()
                            .Setup<bool>("CheckApplication", ItExpr.IsAny<CreditCardApplication>() )
                            .Returns(true);

            var sut = new CreditCardApplicationEvaluator(mockValidator.Object, mockFraudLookup.Object);

            var application = new CreditCardApplication();

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.RefferedToHumanFraudRisk, decision);
        }

        //[Fact]
        //public void LinqToMocks()
        //{
        //    Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>();

        //    mockValidator.Setup(x => x.ServiceInformation.Licence.LicenceKey).Returns((string)"OK");

        //    mockValidator.SetupSequence(x => x.IsValid(It.IsAny<string>())).Returns(true);

        //    var sut = new CreditCardApplicationEvaluator(mockValidator.Object);

        //    var application = new CreditCardApplication { Age = 25 };

        //    CreditCardApplicationDecision decision = sut.Evaluate(application);

        //    Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);

        //}

        [Fact]
        public void LinqToMocks_Refactored()
        {
            IFrequentFlyerNumberValidator mockValidator = Mock.Of<IFrequentFlyerNumberValidator>
                (
                    validator => validator.ServiceInformation.Licence.LicenceKey == "OK" &&
                                 validator.IsValid(It.IsAny<string>())== true
                );

            var sut = new CreditCardApplicationEvaluator(mockValidator);

            var application = new CreditCardApplication { Age = 25 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, decision);

        }
    }
}