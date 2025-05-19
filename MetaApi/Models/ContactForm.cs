using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetaApi.Models
{
    public class ContactForm
    {
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
    }


    public class Vendor
    {
          public string accesskey { get; set; }
          public string BrandName { get; set; }
          public string Email{ get; set; }
          public string Phone{ get; set; } 
          public string Country{ get; set; }
          public string Logo{ get; set; }
          public int CustomerCount{ get; set; }
          public int ProductCount{ get; set; }
          public int SoldCount{ get; set; }
          public string VendorID{ get; set; }
          public string Creator{ get; set; }
          public string keydte{ get; set; }
    }


    public class GiftCards
    {       
          public string accesskey { get; set; }
          public string ProductName { get; set; }
          public string Brand{get; set;}
          public string Description{get; set;}
          public string Amount{get; set;}
          public string Vendor{get; set;}
          public string Available{get; set;}
          public string keydte{get; set;}
          public string CreatorEmail {get; set;}
          public string ImageUrl { get; set; }
          public string VendorShopID { get; set; }
    }

    public class Orders
    {
      public string accesskey { get; set; }
      public string OrderID{get; set;}
      public string Order{get; set;}
      public string Customer{get; set;}
      public string keydte{get; set;}
      public string Items{get; set;}
      public string Payment{get; set;}
      public string Total{get; set;}
      public string OrderStatus{get; set;}
      public string CustomerEmail{get; set;}
      public string Processor{get; set;}
      public string ProcessorEmail{get; set;}
      public string VendorID{get; set;}
    }
}