using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MetaApi.Models;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using Stripe;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using PayPalCheckoutSdk.Orders;
using static MetaApi.Models.VendorInfo;
using Xceed.Words.NET;
using DocumentFormat.OpenXml.Packaging;
using HtmlToOpenXml;
using DocumentFormat.OpenXml;
using AngleSharp.Dom;
using System.Web.Optimization;
using System.Net.Http.Headers;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.RegularExpressions;

namespace MetaApi.Controllers
{
    [AllowAnonymous]
    public class DataController : ApiController
    {
        LocalServices services = new LocalServices();

        [HttpGet]
        [Route("api/Data/tester")]
        [AllowAnonymous]
        public IHttpActionResult FetchStudentDetails()
        {
            return Ok("data is working");

        }

        public string MailTemplates(string content, string header, string email, string subject)
        {
            string msg = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <style>
                                /* Add your CSS styles here */
                                body {{
                                    font-family: 'Georgia', serif;
                                    width: 100%;
                                    background-color: grey;
                                    margin: 20;
                                    padding: 0;
                                    display: flex;
                                    justify-content: center;
                                    align-items: center;
                                }}
                                .container {{
                                    width: 100%;
                                    display: flex;
                                    justify-content: center;
                                    align-items: center;
                                    min-height: 100vh;
           
                                }}
                                .card {{
                                    width: 100%;
                                    background-color: white;
                                    padding: 20px;
                                    border-radius: 15px;
                                    box-shadow: 0px 0px 20px rgba(0, 0, 0, 0.1);
                                    text-align: center;
                                }}
                                .card-header {{
                                    background-color: #000080;
                                    color: #ffffff !important;
                                    padding: 20px;
                                    border-top-left-radius: 15px;
                                    border-top-right-radius: 15px;
                                    font-size: 18px;
                                    margin-bottom: 40px;
                                    font-weight :bold;
                                }}
                                .content {{
                                    margin: 13px;
                                    padding: 10px;
                                    text-align :left !important;
                                }}
                                .content p {{
                                    font-size: 13px;
                                    margin-bottom: 6px;
                                }}
                                .card-footer {{
                                    background-color: #000080;
                                    color: #ffffff !important;
                                    padding: 20px;
                                    border-bottom-left-radius: 15px;
                                    border-bottom-right-radius: 15px;
                                    font-size: 11px;
                                }}
                            </style>
                        </head>
                        <body>
                            <table width='100%' height='100%' cellpadding='0' cellspacing='0' border='0'>
                                    <tr><td align='center' valign='middle' style='padding:10px;'>        
                                     <div class='container'>
                                <div class='card'>
                                    <div class='card-header'>
                                   Shield Guard AI Security Tool
                        <hr>
                                        {header}

                                    </div>
                                    <div class='content'>
                                        {content}
                                        <p>Happy exploring!</p>
                                    </div>
                                    <div class='card-footer'>      
                            Message From<br>
                             Shield Guard AI Security Tool<br>
                            2024   Shield Guard AI Security Tool. All rights reserved.<hr>
                            <p style='text-align: text-justify !important'>
                        This email and any files transmitted with it are confidential and intended solely for the use of the individual or entity to whom they are addressed. If you have received this email in error, please notify the sender immediately and delete the email from your system. Please note that any views or opinions presented in this email are solely those of the author and do not necessarily represent those of the company. Although precautions have been taken to ensure this email is virus-free, the company cannot accept responsibility for any loss or damage arising from the use of this email or attachments. </p>

                                    </div>
                                </div>
                            </div>
                                 </td>
                                </tr>
                            </table></body>
                        </html>";

            // Call the service to send the email asynchronously
            string result = services.MailAsync2(email, subject, msg);

            return result;
        }

        [HttpGet]
        [Route("api/Data/RequestOTP")]
        public IHttpActionResult SendContactForm(string form)
        {
            Random random = new Random();
            // Generate a random 6-digit OTP
            int otp = random.Next(100000, 999999);

            string SendData = services.SqlCommandInformation("Update tblaccount set otp = '" + otp.ToString() + "' where email ='" + form + "'");
            if (SendData == "Successfull")
            {


                string msg = @"<div style='font-size: 16px;'>
                <p>Your OTP Request Code:</p>
                <p style='font-size: 28px; font-weight:bold'>" + otp + @"</p>
            </div>";

                string mail = @"<div>
    <p>Dear Esteemed Member,</p>
    <p>We're sending you this email to authenticate your Login Process request.</p>
    <p style='font-size: 28px; font-weight:bold'>Your OTP code is: <strong> {otp} </strong>.</p>
    <p>Please use this code within the specified time frame to complete your action or transaction.</p>
    <p>If you did not initiate this request, please ignore this email or contact our support team immediately for assistance.</p>
    <p>Thank you for using our services.</p>
    <p>Best regards,</p>
    <p> Shield Guard AI Security.</p>
</div>";
                mail = mail.Replace("{otp}", otp.ToString());

                string result = MailTemplates(mail, "OTP CODE", form, "ONE-TIME CODE SECURITY CHECKS");

                if (result == "Successful")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

            }
            else
            {
                return BadRequest("Error");
            }
        }

        [HttpPost]
        [Route("api/Data/ForgotPassword")]
        public IHttpActionResult ForgotPassword(LoginModel form)
        {
            string result = services.SqlCommandInformation("Update tblaccount set password='" + form.Password + "' where email='" + form.Email + "' and otp = '" + form.OTP + "'");
            if (result == "Successfull")
            {
                return Ok("Password Changed Successfully");
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPost]
        [Route("api/Data/Register")]
        public IHttpActionResult Register(RegisterModel model)
        {
            try
            {
                string check = services.ReadSection("select email from tblaccount where email='" + model.Email + "'");
                if (check == "" || check == "Failed")
                {
                    var newID = System.Guid.NewGuid();
                    string query = "insert into tblaccount(accesskey,[token],[FullName],[email],[Password],[Phone],[Keydate],[OTP],[isActive],role,ispaid,subplan,startdate,enddate) values('" + newID + "','" + newID + "','" + model.FullName + "','" + model.Email + "','" + services.HashPassword(model.Password) + "','" + model.phone + "','" + DateTime.Now.ToShortDateString() + "','*890jhhjfhjfv3#','1','Customer','1','FREE','" + DateTime.Now.ToShortDateString() + "','" + DateTime.Now.AddDays(30) + "')";
                    string result = services.SqlCommandInformation(query);
                    if (result == "Successfull")
                    {
                        //send email for verification 
                        string mail = "Dear " + model.FullName + ", <br><br>We are delighted to welcome you to Shield Guard AI Security! Your account has been successfully created, and we are excited to have you as a part of our security community.<br><br>At Shield Guard AI Security, we strive to provide top-notch security solutions and an exceptional user experience. Our platform offers cutting-edge features and services designed to safeguard your digital assets and protect your privacy. <br><br>To access your account, please visit our website at https://www.shieldguardai.com and click on the Sign In button located in the top-right corner of the page. Enter your username and password, and you'll be ready to explore the Shield Guard AI Security platform.<br><br>If you ever need assistance or have any questions, our dedicated support team is here to help. Feel free to reach out to us at mailto:support@shieldguardai.com, and we'll respond to your inquiries as promptly as possible.<br><br>Thank you for choosing Shield Guard AI Security! We look forward to your active participation in our security community and ensuring the safety of your digital assets.<br><br>Happy exploring!<br><br>Best regards,<br>The Shield Guard AI Security Team.";


                        string mailReply = MailTemplates(mail, "Welcome to Shield Guard AI", model.Email, "Shield Guard AI Registration");

                        // string mailReply = services.MailAsync2(model.Email, "Education Elite Registration", mail);

                        return Ok();

                    }
                    else
                    {
                        return BadRequest("Cannot Process Data Entry at the Moment");
                    }
                }
                else
                {
                    return BadRequest("Email Exist!, Please try Another Email");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Route("api/Data/Loginv1")]
        public IHttpActionResult LoginWithPasswordAsync(LoginModel model)
        {
            try
            {
                string hash = services.ReadSection("select password from tblaccount where email ='"+model.Email+"'");

                bool isMatch = services.VerifyPassword(model.Password, hash);
                if (isMatch)
                {
                    string query = "select token,Fullname,email,role,phone,ispaid from tblaccount where email='" + model.Email + "' and password ='" + hash + " ' and isactive ='1'";

                    string result = services.ReadSection(query);
                    if (result != "Failed")
                    {
                        var newid = Guid.NewGuid().ToString();
                        services.SqlCommandInformation("update tblaccount set token ='" + newid + "' where email='" + model.Email + "' ");

                        DataTable db = new DataTable();
                        using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                        using (var cmd = new SqlCommand(query, con))
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            cmd.CommandType = CommandType.Text;
                            da.Fill(db);
                        }

                        string mail = @"<div>
                        <p>Hello Shield Guard Member,</p>
                        <p>You've signed in to your account at " + DateTime.Now.ToString() + "." + @" If this wasn't you, please <a href='https://www.shieldguardai.com/forgot'>Click Here </a> to reset your password.</p>
                    </div>
                    ";
                                            string mailReply = MailTemplates(mail, "Login Notification", model.Email, "Shield Guard Notification");

                        return Ok(db);


                    }
                    else
                    {
                        return BadRequest("Invalid Login Details");
                    }
                }
                else
                {
                    // Invalid password
                    return BadRequest("Invalid Login Details");
                }

                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
                string enddate = services.ReadSection("select enddate from tblaccount where email='" + model.Email + "'");
                if (enddate == "" || enddate.ToLower() == "failed")
                {
                    services.SqlCommandInformation("update tblaccount  set ispaid='1' , subplan='FREE', startdate='" + DateTime.Now.ToShortDateString() + "' , enddate='" + DateTime.Now.AddDays(30) + "' where email ='" + model.Email + "'");
                }
                else
                {
                    //DateTime endate = Convert.ToDateTime(enddate);
                    //if (endate < DateTime.Now)
                    //{
                    //    services.SqlCommandInformation("update tblaccount  set ispaid='0' where email ='" + model.Email + "'");
                    //    Console.WriteLine("The account has expired.");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("The account is still active.");
                    //}
                }
            }
        }

        [HttpGet]
        [Route("api/Data/AccountVerified")]
        public IHttpActionResult verifyOTP(string oldMail, string email, string phone, string fullname)
        {
            //   string query = "select * from account where email ='" + email + "' and otp ='" + otp + "'";
            string result = services.SqlCommandInformation("update tblaccount set fullname='" + fullname + "' , email ='" + email + "',phone='" + phone + "'  where email='" + oldMail + "'");

            if (result != "Failed")
            {
                return Ok("Login Successfully");
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet]
        [Route("api/Data/verifyOTP")]
        public IHttpActionResult verifyOTP(string email, string otp)
        {
            string query = "select * from tblaccount where email ='" + email + "' and otp ='" + otp + "'";
            string result = services.ReadSection(query);
            if (result != "Failed")
            {
                return Ok("Login Successfully");
            }
            else
            {
                return BadRequest(result);
            }
        }

        public class mailer
        {
            public string messges { get; set; }
            public string accesskey { get; set; }
            public string emailTo { get; set; }
            public string header { get; set; }
            public string id { get; set; }

        }

        public class Sharedmailer
        {
            public string messges { get; set; }
            public string accesskey { get; set; }
            public string emailTo { get; set; }
            public string header { get; set; }
            public string docID { get; set; }
            public string shared { get; set; }



        }

        [HttpPost]
        [Route("api/Data/SendMail")]
        public IHttpActionResult SendMail(mailer info)
        {
            string mailReply = MailTemplates(info.messges, info.header, info.emailTo, "Shield Guard Security Documentations");

            if (mailReply != "Failed")
            {
                return Ok("Mail Sent Successfully");
            }
            else
            {
                return BadRequest("Failed to Send Email Message!");
            }

        }


        [HttpPost]
        [Route("api/Data/SendMailMessage")]
        public IHttpActionResult SendMailMessage(mailer info)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + info.accesskey + "'");
            if (checkers != "Failed")
            {
                string mailReply = MailTemplates(info.messges, info.header, info.emailTo, "Shield Guard Security Documentations");

                if (mailReply != "Failed")
                {
                    return Ok("Mail Sent Successfully");
                }
                else
                {
                    return BadRequest("Failed to Send Email Message!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpPost]
        [Route("api/Data/SaveDoc")]
        public IHttpActionResult SaveDoc(mailer info)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + info.accesskey + "'");
            if (checkers != "Failed")
            {
                var newid = "";
                if(info.id == "" || info.id == null)
                {
                    newid = Guid.NewGuid().ToString();
                }
                else
                {
                    newid = info.id;
                }
                string msg = info.messges.Replace('\'', ' ');
                string datareply = services.SqlCommandInformation("Insert into documenttable(documentid,title,message,keydte,owner,active) values ('" + newid + "','" + info.header + "','" + msg + "','" + DateTime.Now.ToString() + "','" + info.emailTo + "','1')");
                if (datareply != "Failed")
                {
                    return Ok("Docs Saved Successfully");
                }
                else
                {
                    return BadRequest("Failed to save docs Message!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("api/Data/shared")]
        public IHttpActionResult shared(Sharedmailer info)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + info.accesskey + "'");
            if (checkers != "Failed")
            {
                string findemail = services.ReadSection("select email from tblaccount where email = '" + info.shared + "'");
                if (findemail != "Failed")
                {

                    var newid = Guid.NewGuid().ToString();
                    string datareply = services.SqlCommandInformation("Insert into SharedDocs (documentid,title,message,keydte,owner,shared,active) values ('" + info.docID + "','" + info.header + "','" + info.messges + "','" + DateTime.Now.ToString() + "','" + info.emailTo + "','" + info.shared + "','1')");
                    if (datareply != "Failed")
                    {
                        return Ok("Docs Saved Successfully");
                    }
                    else
                    {
                        return BadRequest("Failed to save docs Message!");
                    }

                }
                else
                {
                    return BadRequest("Email User Not Found!");
                }
            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpGet]
        [Route("api/Data/GetDoc")]
        public IHttpActionResult GetDoc(string accesskey, string owner)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string query = "select * from documenttable where owner = '" + owner + "' order by id desc";
                DataTable db = new DataTable();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                using (var cmd = new SqlCommand(query, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.Text;
                    da.Fill(db);
                }
                return Ok(db);

            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        [Route("api/Data/DeleteGetDoc")]
        public IHttpActionResult DeleteGetDoc(string accesskey, string id)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string query = services.SqlCommandInformation("delete from documenttable where id ='" + id + "'");

                if (query != "Failed")
                {
                    return Ok("Successful!");
                }
                else
                {
                    return BadRequest("query");
                }

            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        [Route("api/Data/GetDoc2")]
        public IHttpActionResult GetDoc2(string accesskey, string owner)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string query = "select * from shareddocs where shared = '" + owner + "' order by id desc";
                DataTable db = new DataTable();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                using (var cmd = new SqlCommand(query, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.Text;
                    da.Fill(db);
                }
                return Ok(db);

            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        [Route("api/Data/DeleteGetDocShared")]
        public IHttpActionResult DeleteGetDocShared(string accesskey, string id)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string query = services.SqlCommandInformation("delete from shareddocs where id ='" + id + "'");
                if (query != "Failed")
                {
                    return Ok("Successful!");
                }
                else
                {
                    return BadRequest("Failed");
                }

            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpGet]
        [Route("api/Data/PayNow")]
        public IHttpActionResult PayNow(string accesskey, string id)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {

                string plan = "";
                DateTime enddate = new DateTime();
                if (id == "24.95")
                {
                    plan = "Monthly";
                    enddate = DateTime.Now.AddDays(30);
                }
                else if (id == "249.95")
                {
                    plan = "Yearly";
                    enddate = DateTime.Now.AddDays(365);
                }
                string query = services.SqlCommandInformation("Update tblaccount set ispaid='1', subplan ='" + plan + "', startdate='" + DateTime.Now.ToShortDateString() + "' , enddate = '" + enddate + "' where token = '" + accesskey + "' ");
                if (query != "Failed")
                {
                    return Ok("Successful!");
                }
                else
                {
                    return BadRequest("Failed");
                }

            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpGet]
        [Route("api/Data/GetCustomer")]
        public IHttpActionResult GetCustomer(string accesskey)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string getrole = services.ReadSection("select role from tblaccount where token ='" + accesskey + "'");
                if (getrole.ToLower() == "admin")
                {
                    string query = "select [token],[FullName],[Email],[Phone],[KeyDate],[isActive],[Role],[isPaid],[subPlan],[startDate],[EndDate]  from tblaccount order by id desc ";
                    DataTable db = new DataTable();
                    using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                    using (var cmd = new SqlCommand(query, con))
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.Text;
                        da.Fill(db);
                    }
                    return Ok(db);
                }
                else
                {
                    return BadRequest();
                }

            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        [Route("api/Data/GetDocs")]
        public IHttpActionResult GetDocs(string accesskey)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string getrole = services.ReadSection("select role from tblaccount where token ='" + accesskey + "'");
                if (getrole.ToLower() == "admin")
                {
                    string query = "select * from [DocumentTable] order by id desc ";
                    DataTable db = new DataTable();
                    using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                    using (var cmd = new SqlCommand(query, con))
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.Text;
                        da.Fill(db);
                    }
                    return Ok(db);
                }
                else
                {
                    return BadRequest();
                }

            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpGet]
        [Route("api/Data/GetShared")]
        public IHttpActionResult GetShared(string accesskey)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string getrole = services.ReadSection("select role from tblaccount where token ='" + accesskey + "'");
                if (getrole.ToLower() == "admin")
                {
                    string query = "select * from SharedDocs order by id desc ";
                    DataTable db = new DataTable();
                    using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                    using (var cmd = new SqlCommand(query, con))
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.Text;
                        da.Fill(db);
                    }
                    return Ok(db);
                }
                else
                {
                    return BadRequest();
                }

            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpPost]
        [Route("api/Data/RegisterStaff")]
        public IHttpActionResult RegisterStaff(RegisterModel model)
        {
            try
            {
                string check = services.ReadSection("select email from tblaccount where email='" + model.Email + "'");
                if (check == "" || check == "Failed")
                {
                    var newID = System.Guid.NewGuid();
                    var passkey = new Random().Next(100000, 9999999).ToString();
                    string query = "insert into tblaccount(accesskey,[token],[FullName],[email],[Password],[Phone],[Keydate],[OTP],[isActive],role,ispaid,subplan,startdate,enddate) values('" + newID + "','" + newID + "','" + model.FullName + "','" + model.Email + "','" + model.Password + "','" + model.phone + "','" + DateTime.Now.ToShortDateString() + "','*890jhhjfhjfv3#','1','Admin','1','FREE','" + DateTime.Now.ToShortDateString() + "','" + DateTime.Now.AddDays(30) + "')";
                    string result = services.SqlCommandInformation(query);
                    if (result == "Successfull")
                    {
                        //send email for verification 
                        string mail = "Dear " + model.FullName + ", <br><br>We are excited to welcome you to Shield Guard AI Security! Your staff account has been successfully created, and we are pleased to have you on board.<br><br>Here are your login credentials:<br><br><strong>Username:</strong> " + model.Email + "<br><strong>Password:</strong> " + model.Password + "<br><br>To access your account, please visit our website at <a href='https://www.shieldguardai.com'>https://www.shieldguardai.com</a> and click on the 'Sign In' button at the top-right corner of the page. Enter your username and password to get started.<br><br>If you need any assistance or have questions, our support team is always here to help. You can contact us at <a href='mailto:support@shieldguardai.com'>support@shieldguardai.com</a>, and we'll be happy to assist.<br><br>Thank you for joining Shield Guard AI Security! We look forward to working with you and ensuring the security of our platform.<br><br>Best regards,<br>The Shield Guard AI Security Team.";


                        string mailReply = MailTemplates(mail, "Welcome to Shield Guard AI", model.Email, "Shield Guard AI Registration");

                        // string mailReply = services.MailAsync2(model.Email, "Education Elite Registration", mail);

                        return Ok();

                    }
                    else
                    {
                        return BadRequest("Cannot Process Data Entry at the Moment");
                    }
                }
                else
                {
                    return BadRequest("Email Exist!, Please try Another Email");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("api/Data/DeleteAccount")]
        public IHttpActionResult DeleteAccount(string accesskey, string email)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string query = services.SqlCommandInformation("delete from tblaccount where email ='" + email + "'");
                if (query != "Failed")
                {
                    return Ok("Successful!");
                }
                else
                {
                    return BadRequest("Failed");
                }

            }
            else
            {
                return Unauthorized();
            }
        }


        private const string StripeSecretKey = "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"; // Replace 'sk_test_XXXXXX' with your Stripe secret key
        public class PaymentRequestData
        {
            public string Token { get; set; }
            public long Amount { get; set; }
        }

        [HttpPost]
        [Route("api/Data/StripeCharge2")]
        public IHttpActionResult ProcessPayment([FromBody] PaymentRequestData requestData)
        {
            try
            {
                //testc keys : sk_test_51NLbbbGPnfheVAhxFNI2tgm0EW8gD1RefHh7F3EGIRIAPJ787WSOtVUXSyHIQnSLOyp7blv5FeNV9UJHPfKYhvJV00zpVqB7om
                //  string stripeSecretKey = "YOUR_STRIPE_SECRET_KEY"; // Replace this with your actual Stripe secret key
                Stripe.StripeConfiguration.ApiKey = StripeSecretKey;

                if (requestData == null)
                {
                    return BadRequest("Invalid request data.");
                }

                if (string.IsNullOrEmpty(requestData.Token))
                {
                    return BadRequest("Token is required.");
                }

                if (requestData.Amount <= 0)
                {
                    return BadRequest("Amount must be greater than zero.");
                }

                string stripeToken = requestData?.Token;
                long amount = requestData?.Amount ?? 0;

                // Use Stripe API to process payment using the token and amount
                var options = new Stripe.ChargeCreateOptions
                {
                    Amount = amount,
                    Currency = "EUR",
                    Source = stripeToken,
                    Description = "Payment for your ShieldGuard Subscription Services",
                    // Add other optional parameters if needed
                };

                var service = new Stripe.ChargeService();
                var charge = service.Create(options);

                // Handle the payment status response from Stripe
                if (charge.Paid)
                {
                    // Payment successful
                    return Ok(new { Message = "Payment successful!" });
                }
                else
                {
                    // Payment failed
                    return BadRequest("Payment failed." + charge.FailureMessage);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions or errors
                return InternalServerError(ex);
            }
        }

        public class Message
        {
            public string messages { get; set; }
        }
        [HttpPost]
        [Route("api/Data/GetWord")]
        public IHttpActionResult DownloadDocument(Message Msg)
        {
            using (var stream = new MemoryStream())
            {
                using (var wordDoc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                    mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(new Body());

                    // Optional but recommended: add styles/numbering support
                    var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
                    stylesPart.Styles = new DocumentFormat.OpenXml.Wordprocessing.Styles(); // You can load default styles here

                    var converter = new HtmlConverter(mainPart);
                    converter.ParseHtml(SanitizeHtml(Msg.messages)); // Sanitize if needed

                    mainPart.Document.Save();
                }

                stream.Position = 0;
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(stream.ToArray())
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "ShieldGuardPlan.docx"
                };

                return ResponseMessage(result);
            }

        }

        string SanitizeHtml(string html)
        {
            // Basic cleanup: remove scripts and unsupported tags
            return Regex.Replace(html, @"<script[^>]*>[\s\S]*?</script>", "", RegexOptions.IgnoreCase);
        }

        public class Project
        {
            // Properties
            public string accesskey { get; set; }
            public int ProjectID { get; set; }
            public string ProjectName { get; set; }
            public string FloorPlan { get; set; }
            public string Regulations { get; set; }
        }

        [HttpPost]
        [Route("api/Data/CreateProject")]
        public IHttpActionResult CreateProject(Project proj)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + proj.accesskey + "'");
            if (checkers != "Failed")
            {
                string getrole = services.ReadSection("select email from tblaccount where token ='" + proj.accesskey + "'");
                var newID = System.Guid.NewGuid();
                string result = services.SqlCommandInformation("insert into project (projectid,projectname,floorplan,regulations,keydte,Email) values  ('" + newID + "','" + proj.ProjectName + "', '" + proj.FloorPlan + "','" + proj.Regulations + "','" + DateTime.Now.ToShortDateString() + "','"+getrole+"')");

                if (result != "Failed")
                {
                    return Ok("Successful");
                }
                else
                {
                    return BadRequest(result);
                }

            }
            else
            {
                return BadRequest();

            }
        }


        [HttpGet]
        [Route("api/Data/SavedProject")]
        public IHttpActionResult SavedProject(string accesskey)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string getrole = services.ReadSection("select email from tblaccount where token ='" + accesskey + "'");
              
                    string query = @"SELECT 
                                            P.ProjectID AS ProjectId,
                                            P.ProjectName,
                                            P.FloorPlan,
                                            P.Regulations,
                                            P.keydte AS ProjectEntryDate,
                                            COUNT(D.id) AS NumberOfFiles
                                        FROM
                                            Project P
                                        LEFT JOIN
                                            DocumentTable D
                                        ON
                                            P.ProjectID = D.documentid
                                         where P.Email = '" + getrole+ @"'
                                        GROUP BY
                                            P.ProjectID, P.ProjectName, P.FloorPlan, P.Regulations, P.keydte
                                        ORDER BY
                                            P.keydte DESC ";


                    DataTable db = new DataTable();
                    using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                    using (var cmd = new SqlCommand(query, con))
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        cmd.CommandType = CommandType.Text;
                        da.Fill(db);
                    }
                    return Ok(db);
               

            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpGet]
        [Route("api/Data/ComProject")]
        public IHttpActionResult ComProject(string accesskey,string projectID)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string getrole = services.ReadSection("select email from tblaccount where token ='" + accesskey + "'");

                string query = @"Select * from project where projectid ='"+projectID+"' ";

                DataTable db = new DataTable();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                using (var cmd = new SqlCommand(query, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.Text;
                    da.Fill(db);
                }
                return Ok(db);

            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpGet]
        [Route("api/Data/GetList")]
        public IHttpActionResult GetList(string accesskey, string projectID)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string getrole = services.ReadSection("select email from tblaccount where token ='" + accesskey + "'");

                string query = @"Select * from documenttable where documentid ='" + projectID + "' ";

                DataTable db = new DataTable();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                using (var cmd = new SqlCommand(query, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.Text;
                    da.Fill(db);
                }
                return Ok(db);

            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpGet]
        [Route("api/Data/GetEmp")]
        public IHttpActionResult GetEmp(string accesskey)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string getrole = services.ReadSection("select email from tblaccount where token ='" + accesskey + "'");

                string query = "select * from tblEmployee where token = '" + getrole + "' order by id desc";
                DataTable db = new DataTable();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                using (var cmd = new SqlCommand(query, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.Text;
                    da.Fill(db);
                }
                return Ok(db);

            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpPost]
        [Route("api/Data/saveEmp")]
        public IHttpActionResult saveEmp(Employee proj)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + proj.accesskey + "'");
            if (checkers != "Failed")
            {
                string getrole = services.ReadSection("select email from tblaccount where token ='" + proj.accesskey + "'");
                var newID = System.Guid.NewGuid();

                string query = @"
            INSERT INTO [tblEmployee]
           (id,[token]
           ,[Fullname]
           ,[Email]
           ,[Position]
           ,[Department]
           ,[JoinDate]
           ,[Phone]
           ,[keydte])
           VALUES
           ('"+newID+"','" + getrole + "','" + proj.FullName + "', '" + proj.Email + "','" + proj.Position + "','" + proj.Department + "','" +  proj.JoinDate + "','" + proj.Phone + "','" + DateTime.Now.ToShortDateString() + "');";

                string result = services.SqlCommandInformation(query);

                if (result != "Failed")
                {
                    return Ok("Successful");
                }
                else
                {
                    return BadRequest(result);
                }

            }
            else
            {
                return BadRequest();

            }
        }



        [HttpGet]
        [Route("api/Data/DeleteEmp")]
        public IHttpActionResult DeleteEmp(string accesskey , string id)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string result = services.SqlCommandInformation("delete from tblemployee where id ='"+id+"'");

                if (result != "Failed")
                {
                    return Ok("Successful");
                }
                else
                {
                    return BadRequest(result);
                }

            }
            else
            {
                return BadRequest();

            }
        }



        [HttpPost]
        [Route("api/Data/saveAssets")]
        public IHttpActionResult saveAssets(assets proj)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + proj.accesskey + "'");
            if (checkers != "Failed")
            {
                string getrole = services.ReadSection("select email from tblaccount where token ='" + proj.accesskey + "'");

                var newID = System.Guid.NewGuid();

                string query = @"INSERT INTO [tblAssets]
                       ([Name]
                       ,[Category]
                       ,[Description]
                       ,[PurchasedDate]
                       ,[Value]
                       ,[Status]
                       ,[Location]
                       ,[Assigned_To]
                       ,[keydate], email)
                       VALUES
                       ('" + proj.Name + "','" + proj.Category + "', '"+proj.Description+"', '" + proj.PurchasedDate + "','" + proj.Value + "','" + proj.Status + "','" + proj.Location + "','" + proj.AssignedTo + "','" + DateTime.Now.ToShortDateString() + "','"+getrole+"');";

                string result = services.SqlCommandInformation(query);

                if (result != "Failed")
                {
                    return Ok("Successful");
                }
                else
                {
                    return BadRequest(result);
                }

            }
            else
            {
                return BadRequest();

            }
        }

        [HttpGet]
        [Route("api/Data/GetAssets")]
        public IHttpActionResult GetAssets(string accesskey)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string getrole = services.ReadSection("select email from tblaccount where token ='" + accesskey + "'");

                string query = "select * from tblAssets where email = '" + getrole + "' order by id desc";
                DataTable db = new DataTable();
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString))
                using (var cmd = new SqlCommand(query, con))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.Text;
                    da.Fill(db);
                }
                return Ok(db);

            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        [Route("api/Data/DeleteAssets")]
        public IHttpActionResult DeleteAssets(string accesskey, string id)
        {
            string checkers = services.ReadSection("select token from tblaccount where token ='" + accesskey + "'");
            if (checkers != "Failed")
            {
                string result = services.SqlCommandInformation("delete from tblassets where id ='" + id + "'");

                if (result != "Failed")
                {
                    return Ok("Successful");
                }
                else
                {
                    return BadRequest(result);
                }

            }
            else
            {
                return BadRequest();

            }
        }

    }
}