using System;

namespace CreditCardApplications
{
    public class CreditCardApplicationEvaluator
    {
        //for tests
        private readonly IFrequentFlyerNumberValidator _validator;

        private const int AutoReferralMaxAge = 20;
        private const int HighIncomeThreshold = 100_000;
        private const int LowIncomeThreshold = 20_000;

        //for tests
            public CreditCardApplicationEvaluator(IFrequentFlyerNumberValidator validator)
            {
                _validator=validator ?? throw new System.ArgumentNullException(nameof(validator));
            }

        public CreditCardApplicationDecision Evaluate(CreditCardApplication application)
        {
            if (application.GrossAnnualIncome >= HighIncomeThreshold)
            {
                return CreditCardApplicationDecision.AutoAccepted;
            }

            //for testing properties
            //_validator.LicenceKey == "EXPIRED"
            if (_validator.ServiceInformation.Licence.LicenceKey == "EXPIRED")
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            //for testing ValidationMode
            _validator.ValidationMode = application.Age >=30 ? ValidationMode.Deatailed : ValidationMode.Quick;

            bool isValidFrequentFlyerNumber;

            try
            {
                //for tests
                isValidFrequentFlyerNumber = _validator.IsValid(application.FrequentFlyerNumber);
            }
            catch (Exception)
            {
                //log
                return CreditCardApplicationDecision.ReferredToHuman;
            }



            //for tests
                if( !isValidFrequentFlyerNumber)
                {
                    return CreditCardApplicationDecision.ReferredToHuman;
                }



            if (application.Age <= AutoReferralMaxAge)
            {
                return CreditCardApplicationDecision.ReferredToHuman;
            }

            if (application.GrossAnnualIncome < LowIncomeThreshold)
            {
                return CreditCardApplicationDecision.AutoDeclined;
            }

     

            return CreditCardApplicationDecision.ReferredToHuman;
        }

        //public CreditCardApplicationDecision EvaluateUsingOut(CreditCardApplication application)
        //{
        //    if (application.GrossAnnualIncome >= HighIncomeThreshold)
        //    {
        //        return CreditCardApplicationDecision.AutoAccepted;
        //    }

        //    _validator.IsValid(application.FrequentFlyerNumber,
        //                       out var isValidFrequentFlyerNumber);

        //    if (!isValidFrequentFlyerNumber)
        //    {
        //        return CreditCardApplicationDecision.ReferredToHuman;
        //    }

        //    if (application.Age <= AutoReferralMaxAge)
        //    {
        //        return CreditCardApplicationDecision.ReferredToHuman;
        //    }

        //    if (application.GrossAnnualIncome < LowIncomeThreshold)
        //    {
        //        return CreditCardApplicationDecision.AutoDeclined;
        //    }

        //    return CreditCardApplicationDecision.ReferredToHuman;
        //}
    }
}
