using PayPalCheckoutSdk.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetaApi.Models
{
    public class VendorInfo
    {
        public class CreateStaff
        {
            public string esamekey { get; set; }
            public string Fullname { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Phone { get; set; }
            public string Department { get; set; }
        }

        public class ProductCategory
        {
            public string accesskey { get; set; }
            public string productCategoryID { get; set; }
            public string ProductCategoryName { get; set; }
        }

        public class GreetingCards
        {
            public string Category { get; set; }
            public string PhotoUrl { get; set; }
            public string VendorShop { get; set; }
            public string CreatorEmail { get; set; }
            public string CreatorAccesskey { get; set; }

        }

        public class OrdersModel
        {
            //public string accesskey { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Country { get; set; }
            public string Role { get; set; }
            public string Department { get; set; }
            public string isActive { get; set; }


        }


        public class DeleteModel
        {
            public string accesskey { get; set; }
            public string VendorID { get; set; }
            public string StaffName { get; set; }
            public string OrdID { get; set; }
            public string CustomerName { get; set; }
            public string ProductName { get; set; }
            public string Quantity { get; set; }
            public string ProductTotal { get; set; }


        }

        public class CustomerModel
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string ItemSold { get; set; }
            public string TotalSpent { get; set; }
        }

        public class PaymentModel
        {
            public string OrderId { get; set; }
            public string CustomerEmail { get; set; }
            public string CustomerName { get; set; }
            public string TransactionAmt { get; set; }
            public string TransactionType { get; set; }
            public string AttendantName { get; set; }
            public string VendorShop { get; set; }
            public DateTime TransDate { get; set; }
            public string TransStatus { get; set; }
            public string TransactionRef { get; set; }

        }

        public class ActivateModel
        {
            public string accesskey { get; set; }
            public string Email { get; set; }
            public string serial { get; set; }
            public string pin { get; set; }
        }

        public class ProfileModel
        {
            public string ApplicationNo { get; set; }
            public string CenterNo { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string BusinessName { get; set; }
            public string BusinessEmail { get; set; }
            public string BusinessPhone { get; set; }
            public string LGA { get; set; }
            public string CenterType { get; set; }
            public string isPaid { get; set; }
            public string PaymentProof { get; set; }
            public string isApproval { get; set; }
            public string Status { get; set; }
            public string keydte { get; set; }
            public string Officer { get; set; }
            public string OfficerName { get; set; }
            public string OfficerPhone { get; set; }

        }

        public class StaffModel
        {
            public List<ProfileModel> Details { get; set; }
            public string Customers { get; set; }
            public string pendingRequest { get; set; }
            public string ApprovalRequest { get; set; }
            public string StaffLists { get; set; }


        }

        public class Claims
        {
            public string accesskey { get; set; }
            public string Requester { get; set; }
            public string RequesterID { get; set; }
            public string RequesterEmail { get; set; }
            public string Messages { get; set; }
            public string amount { get; set; }
        }

        public class BioReport
        {
            public string PaymentDate { get; set; }
            public string NextPaymentDate { get; set; }
            public List<OrdersModel> Details { get; set; }
            public List<ProfileModel> ApplicationHistory { get; set; }
            public List<History> transaction { get; set; }

        }

        public class History
        {
            public string ApplicationNumber { get; set; }
            public string Applicantname { get; set; }
            public string ApplicantEmail { get; set; }
            public string Notes { get; set; }
            public string Amount { get; set; }
            public string Status { get; set; }
            public string Officer { get; set; }
            public DateTime keydte { get; set; }
            public string Year { get; set; }

        }


        public class Dashboard
        {
            public string vendor { get; set; }
            public string cards { get; set; }
            public string customers { get; set; }
            public string claims { get; set; }

            public List<Claims> Dataset { get; set; }
        }


        public class Reporters
        {
            public List<OrdersModel> Details { get; set; }
            public string DailySales { get; set; }
            public string Instore { get; set; }
            public string OnlineSales { get; set; }
            public string totalOrders { get; set; }
            public string CardBalance { get; set; }
            public string mylocalecard { get; set; }

        }

        public class PayRequest
        {
            public string accesskey { get; set; }
            public string account { get; set; }
            public string amount { get; set; }
            public string description { get; set; }
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
        }

        public class Loan
        {
            public string accesskey { get; set; }
            public string LoanAmount { get; set; }
            public string Description { get; set; }
            public string LoanType {get;set;}
            public string  User1 { get; set; }
            public string User2 { get; set; }
            public string User3 { get; set; }
            public string CustomerName { get; set; }
            public string CustomerEmail { get; set; }
            public string Loanref { get; set; }
            public string LoanDuration { get; set; }
            public string WhatYouGet { get; set; }
            public string LoanInterest { get; set; }
            public string Checks { get; set; }

        }

        public class Chat
        {
            public string accesskey { get; set; }
            public string msgref { get;  set; }
            public string sender { get; set; }
            public string email { get; set; }
            public string reciever { get; set; }
            public string message { get; set; }
            public string code { get; set; }
        }
        public class PayPalClient
        {
            public static PayPalEnvironment environment()
            {
                return new SandboxEnvironment("s4109361_api1.gmail.com", "ZXWVUMTA9AVYJEYHAB.zLWpY0PWM.Im9xAO7o3KC - LfQADroc9kbDf2d91T7zYGgdG5a5mPs");
            }

            public static PayPalHttpClient client()
            {
                return new PayPalHttpClient(environment());
            }
        }


        public class CaptureResult
        {
            public string id { get; set; }
            public string status { get; set; }
            public List<PurchaseUnit> purchase_units { get; set; }
            public List<RelatedResources> related_resources { get; set; }
            public string create_time { get; set; }
            public string update_time { get; set; }
        }

        public class PurchaseUnit
        {
            public AmountWithBreakdown amount { get; set; }
            public Payee payee { get; set; }
        }

        public class AmountWithBreakdown
        {
            public string currency_code { get; set; }
            public string value { get; set; }
        }

        public class Payee
        {
            public string email_address { get; set; }
        }

        public class RelatedResources
        {
            public Sale sale { get; set; }
        }

        public class Sale
        {
            public string id { get; set; }
            public Amount amount { get; set; }
            public string status { get; set; }
        }

        public class Amount
        {
            public string currency_code { get; set; }
            public string value { get; set; }
        }


        public class dashboard
        {
            public string TotalCustomers { get; set; }
            public string TotalAdmin { get; set; }
            public string BasicLoan { get; set; }
            public string BufferLoan { get; set; }
            public string RBumperLoan { get; set; }
            public string STBumperLoan { get; set; }
            public string CountBasicLoan { get; set; }
            public string CountBufferLoan { get; set; }
            public string CountRBumperLoan { get; set; }
            public string CountSTBumperLoan { get; set; }
            public string Elite { get; set; }
            public string Smart { get; set; }
            public string Wallet { get; set; }
            public string Equity { get; set; }
            public string InterestPayable { get; set; }
            public string InterestCR { get; set; }
            public string InterestDR { get; set; }



        }

        public class Employee
        {
            public int Id { get; set; }
            public string accesskey { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Position { get; set; }
            public string Department { get; set; }
            public string JoinDate { get; set; }
            public string Phone { get; set; }
            public DateTime KeyDate { get; set; }
        }

        public class assets
        {
            public string accesskey { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public string Description { get; set; }
            public DateTime PurchasedDate { get; set; }
            public decimal Value { get; set; }
            public string Status { get; set; }
            public string Location { get; set; }
            public string AssignedTo { get; set; }
            public DateTime KeyDate { get; set; }
            public string Email { get; set; }

        }


    }
}