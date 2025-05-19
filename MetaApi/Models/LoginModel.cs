using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MetaApi.Models
{
        public class LoginModel
        {

            public string Email { get; set; }
            public string Password { get; set; }
            public string OTP { get; set; }
        }

        public class RegisterModel
        {

            [Required(ErrorMessage = "FullName Required")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Email Required")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password Required")]
            public string Password { get; set; }
            public string phone { get; set; }
            public string address { get; set; }
        public string dob { get; set; }
        public string accountname { get; set; }
        public string accountnumber { get; set; }
        public string bankname { get; set; }
        public string state { get; set; }
        public string lga { get; set; }
        public string mda { get; set; }
        public string oracle { get; set; }
        public string DOFA { get; set; }
        public string DOR { get; set; }
        public string accesskey { get; set; }
        public string check { get; set; }
        }

        public class SaveHistory
        {
            public string accesskey { get; set; }
            public string imageurl { get; set; }
            public string prompts { get; set; }
        }

    public class AccountBalance    {
        public string wallet { get; set; }
        public string equitycapital { get; set; }
        public string elitesavings { get; set; }
        public string smartsavings { get; set;  }
        public string deductions { get; set; }
        public string basicloan { get; set; }
        public string bufferloan { get; set; }
        public string bumperloan { get; set; }
        public string interestpayable { get; set; }
        public string reducebumperloan { get; set; }
    }


    public class transPosting
    {
        public string accesskey { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public string Description { get; set; }
        public string transType { get; set; }
        public string payType { get; set; }
        public string AccountType { get; set; }
        public string Payeer { get; set; }
        public string transAmount { get; set; }
        public string sendingAccount { get; set; }
        public string recieveingAccount { get; set; }
    }


}