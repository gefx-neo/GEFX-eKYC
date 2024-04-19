namespace ekyc
{
    public class NaturalCustomer
    {
        public class Crp
        {
            public bool active { get; set; }
            //public Forms forms { get; set; }
           // public int id { get; set; }
           // public int profileId { get; set; }
            public string profileReferenceId { get; set; }
            //public string referenceId { get; set; }
            public List<Role> roles { get; set; }
            public string type { get; set; }
            public CrpParticulars particular { get; set; }
            public CrpOther other { get; set; }
        }

        public class CrpOther
        {
            public string status { get; set; }
            public string sourceOfFunds { get; set; }
            public string otherSourceOfFundsValid { get; set; }
            public List<string> bankAccountNumber { get; set; }
            public string undischargedBankrupt { get; set; }
            public string entityType { get; set; }
            public string otherEntityType { get; set; }
            public string corporateWebsite { get; set; }
            public string ownershipStructureLayer { get; set; }
            public string variableCapitalCompany { get; set; }
            public string businessCessationDate { get; set; }
        }

        public class CustomerOther
        {
            public string occupation { get; set; }
            public string onBoardingMode { get; set; }
            public string industry { get; set; }
            public List<string> paymentMode { get; set; }
            public string productServiceComplexity { get; set; }
            public string sourceOfFunds { get; set; }
            public string natureOfBusinessRelationship { get; set; }
            public List<string> bankAccount { get; set; }
            public string additionalInformation { get; set; }
        }

        public class Customer
        {
            public bool active { get; set; }
            public int assigneeId { get; set; }
            //public int batchUploadId { get; set; }
            //public Forms forms { get; set; }
            //public int id { get; set; }
            //public int profileId { get; set; }
            public string profileReferenceId { get; set; }
            public string type { get; set; }        
            public List<int> domainId { get; set; }
            public CustomerOther other { get; set; }
            public CustomerParticulars particular { get; set; }
        }

        public class CrpParticulars
        {
            public string incorporated { get; set; }
            public string name { get; set; }
            public List<string> alias { get; set; } 
            public string formerName { get; set; }
            public string incorporateNumber { get; set; }
            public List<string> countryOfOperation { get; set; }
            public string countryOfIncorporation { get; set; }
            public string dateOfIncorporation { get; set; }
            public string imonumber { get; set; }
            public List<string> address { get; set; }
            public List<string> phone { get; set; }
            public List<string> email { get; set; }
        }

        public class CustomerParticulars
        {
            public string name { get; set; }
            public List<string> alias { get; set; }
            public List<string> formerName { get; set; }
            public string salutation { get; set; }
            public List<string> address { get; set; }
            public List<string> phone { get; set; }
            public List<string> email { get; set; }
            public string gender { get; set; }
            public string countryOfResidence { get; set; }
            public string countryOfBirth { get; set; }
            public string dateOfBirth { get; set; }
            public List<string> nationality { get; set; }
        }


        public class Forms
        {
        }

        public class Role
        {
            public string appointedDate { get; set; }
            public string resignedDate { get; set; }
            public string role { get; set; }
        }

            public List<Crp> crps { get; set; }
            public Customer customer { get; set; }
            public bool triggerScreening { get; set; }
            public bool triggerScreeningForCrp { get; set; }

    }
}
