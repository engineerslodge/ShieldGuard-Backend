using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MetaApi.Controllers
{
    public class LocalServices
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ToString();

        public OleDbConnection oledbConnection;
        public OleDbCommand oledbcommand;

        public string Login(string Username, string Password)
        {
            string Jasonresult = string.Empty;
            SqlDataReader reader = null;
            SqlConnection connection = new SqlConnection(ConnectionString);
            string Selectquery = "Select Fullname,Phone,Email,systemrole,orgID from Account Where Email =@username And Password=@password And Isnull(Active,'0') ='1'";

            try
            {
                connection.Open();

                string fullname = "";
                string Phone = "";
                string email = "";
                string systemrole = "";
                string KeyID = "";
                SqlCommand cmd = new SqlCommand(Selectquery, connection);
                cmd.Parameters.AddWithValue("username", Username.Trim());
                cmd.Parameters.AddWithValue("password", Password.Trim());
                reader = cmd.ExecuteReader();
                if (reader.HasRows == true)
                {
                    while (reader.Read())
                    {
                        fullname = reader["FullName"].ToString();
                        Phone = reader["Phone"].ToString();
                        email = reader["Email"].ToString();
                        systemrole = reader["SystemRole"].ToString();
                        KeyID = reader["OrgID"].ToString();
                    }

                    Jasonresult = "Successfull:" + fullname + ":" + Phone + ":" + email + ":" + systemrole + ":" + KeyID;

                }
                else
                {
                    Jasonresult = "Failed";
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                Jasonresult = "Failed:" + ex.Message;


            }

            finally
            {
                connection.Close();
            }

            return Jasonresult;

        }

        public string AccountRegistration(string Update_insert_code, string fullname, string email, string phone, string password, bool deleted, bool suspended, string SystemRole, string active, string orgname)
        {
            string Jasonresult = string.Empty;
            SqlDataReader reader = null;
            SqlConnection connection = new SqlConnection(ConnectionString);
            string insertQuery = "INSERT INTO [dbo].[Account] ([Fullname],[Phone],[Email],[password],[systemRole],[deleted],[suspended],[systemdate],[Active],[orgID]) Values ('" + fullname + "','" + phone + "','" + email + "','" + password + "','" + SystemRole + "','" + deleted + "','" + suspended + "','" + DateTime.Now.ToShortDateString().ToLower() + "','" + active + "','" + orgname + "')";

            string forgotpassword = "update account set password= '" + password + "' where email = '" + email + "'";
            connection.Open();
            try
            {
                if (Update_insert_code == string.Empty)
                {
                    int q = new SqlCommand(insertQuery, connection).ExecuteNonQuery();
                    if (q > 0)
                    {
                        Jasonresult = "Successful";
                    }
                    else
                    {
                        Jasonresult = "You have may Have to try Again ! , Account Creation Not Successful ";
                    }
                }
                else
                {
                    int r = new SqlCommand(forgotpassword, connection).ExecuteNonQuery();
                    if (r > 0)
                    {
                        Jasonresult = "Successful";
                    }
                    else
                    {
                        Jasonresult = "You have may Have to try Again ! , Account Update Not Successful ";
                    }
                }

            }
            catch (Exception ex)
            {
                Jasonresult = ex.Message + "\n Error Please Contact Administrator";
            }
            connection.Close();
            return Jasonresult;
        }

        public void LoadGrid(GridView grid, string query)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            grid.DataSource = dt;
            grid.DataBind();

            connection.Close();



        }

        public string BulkSms(string username, string password, string sender, string messages, string mobileNumber)
        {
            string Jasonresult = string.Empty;
            try
            {

                string url = "https://sms.engineerslodge.com.ng/api/?username=" + username + "&password=" + password + "&sender=" + sender + "&message=" + messages + "&mobiles=" + mobileNumber;

                //HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(string.Format(url));
                //webReq.Method = "GET";
                //HttpWebResponse webResponse = (HttpWebResponse)webReq.GetResponse();
                //Stream answer = webResponse.GetResponseStream();
                //StreamReader _recivedAnswer = new StreamReader(answer);

                StreamReader reader = new StreamReader(WebRequest.Create(url).GetResponse().GetResponseStream());
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Jasonresult = line;
                }




            }
            catch (Exception ex)
            {
                Jasonresult = "Failed : " + ex.Message;
            }

            return Jasonresult;
        }

        public string LoginBulkSMS(string username, string password)
        {
            string Jasonresult = string.Empty;
            try
            {

                string url = "https://sms.engineerslodge.com.ng/api/?username=" + username + "&password=" + password + "&action=login";

                //HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(string.Format(url));
                //webReq.Method = "GET";
                //HttpWebResponse webResponse = (HttpWebResponse)webReq.GetResponse();
                //Stream answer = webResponse.GetResponseStream();
                //StreamReader _recivedAnswer = new StreamReader(answer);

                StreamReader reader = new StreamReader(WebRequest.Create(url).GetResponse().GetResponseStream());
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Jasonresult = line;
                }


            }
            catch (Exception ex)
            {
                Jasonresult = "Failed : " + ex.Message;
            }

            return Jasonresult;
        }
        public string Balance(string username, string password)
        {

            string Jasonresult = string.Empty;
            try
            {

                string url = "http://sms.engineerslodge.com.ng/api/?username=" + username + "&password=" + password + "&action=balance";

                //HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(string.Format(url));
                //webReq.Method = "GET";
                //HttpWebResponse webResponse = (HttpWebResponse)webReq.GetResponse();
                //Stream answer = webResponse.GetResponseStream();
                //StreamReader _recivedAnswer = new StreamReader(answer);

                StreamReader reader = new StreamReader(WebRequest.Create(url).GetResponse().GetResponseStream());
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Jasonresult = line;
                }


            }
            catch (Exception ex)
            {
                Jasonresult = "Failed : " + ex.Message;
            }

            return Jasonresult;
        }
        public void LoadNotify(DataList grid, string query)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            grid.DataSource = dt;
            grid.DataBind();
            connection.Close();
        }
        public void LoadDropdownlist(DropDownList grid, string query, string datas)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            SqlCommand cmd = new SqlCommand(query, connection);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            grid.DataSource = dt;
            grid.DataMember = datas;
            grid.DataTextField = datas;
            grid.DataBind();
            connection.Close();
        }
        public string SqlCommandInformation(string query)
        {
            string Jasonresult = string.Empty;
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            int r = new SqlCommand(query, connection).ExecuteNonQuery();
            if (r > 0)
            {
                Jasonresult = "Successfull";
            }
            else
            {
                Jasonresult = "Failed";
            }


            return Jasonresult;

        }
        public string UpdateAccountRegistartion(string fullname, string email, string phone, string active, string orgname, string id)
        {
            string Jasonresult = string.Empty;
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            string query = "UPDATE [dbo].[Account] SET [Fullname] = '" + fullname + "', [Phone] = '" + phone + "', " +
                "Email ='" + email + "'," + "active ='" + active + "' where id ='" + id + "'";

            int r = new SqlCommand(query, connection).ExecuteNonQuery();
            if (r > 0)
            {
                Jasonresult = "Successfull";
            }
            else
            {
                Jasonresult = "Failed";
            }


            return Jasonresult;

        }
        public string SchoolAccount(string datacode, string schoolname, string schoolphone, string schoolemail, string schooladdress, string contactname, string createdby, bool flags, string schoollogo, string schoolcode)
        {
            string orgid = "";
            string compact = "";
            SqlTransaction trans = null;
            SqlConnection connection = new SqlConnection(ConnectionString);
            string Jasonresult = string.Empty;
            try
            {
                if (datacode == string.Empty)
                {



                    connection.Open();
                    string selectCount = "select count(*) from School";
                    SqlDataReader reader = new SqlCommand(selectCount, connection, trans).ExecuteReader();
                    if (reader.Read())
                    {

                        compact = reader[0].ToString();
                        orgid = "RPT-365-" + DateTime.Today.Year.ToString() + "-" + compact + DateTime.Now.Second.ToString();
                        reader.Close();

                        string insertQuery = "INSERT INTO [dbo].[School] ([SchoolName],[SchoolEmail],[SchoolPhone],[SchoolAddress],[ContactName],[CreatedBy],[OrgID],[Flag],schoollogo,schoolcode) Values ('" + schoolname + "','" + schoolemail + "','" + schoolphone + "','" + schooladdress + "','" + contactname + "','" + createdby + "','" + orgid + "','" + flags + "','" + schoollogo + "','" + schoolcode + "')";
                        trans = connection.BeginTransaction();
                        int cmd = new SqlCommand(insertQuery, connection, trans).ExecuteNonQuery();
                        if (cmd > 0)
                        {
                            string data = "INSERT INTO [dbo].[Account] ([Fullname],[Phone],[Email],[password],[systemRole],[deleted],[suspended],[systemdate],[Active],[orgID]) Values ('" + contactname + "','" + schoolphone + "','" + schoolemail + "','" + schoolphone + "','Client','0','0','" + DateTime.Now.ToShortDateString().ToLower() + "','1','" + orgid + "')";

                            int r = new SqlCommand(data, connection, trans).ExecuteNonQuery();
                            if (r > 0)
                            {
                                string locking = "Insert into locktable (schoolid,active) values ('" + orgid + "','0')";
                                int ce = new SqlCommand(locking, connection, trans).ExecuteNonQuery();
                                if (ce > 0)
                                {
                                    trans.Commit();
                                    Jasonresult = "Successfull";
                                }



                            }
                            else
                            {
                                trans.Rollback();
                            }
                        }
                        else
                        {
                            trans.Rollback();
                        }
                    }

                    else
                    {
                        trans.Rollback();
                        Jasonresult = "An Error Occurred !";
                    }
                    connection.Close();
                }
                else
                {
                    connection.Open();
                    trans = connection.BeginTransaction();
                    string update = "Update School  SET Schoolname ='" + schoolname + "' , schoolemail ='" + schoolemail + "' , schooladdress ='" + schooladdress + "' , contactname ='" + contactname + "'where id = '" + datacode + "'";
                    int r = new SqlCommand(update, connection, trans).ExecuteNonQuery();
                    if (r > 0)
                    {
                        // update sc
                        string UpdateAccount = "Update Account Set Fullname ='" + contactname + "', Password  ='" + schoolphone + "' where email ='" + schoolemail + "' ";

                        int s = new SqlCommand(UpdateAccount, connection, trans).ExecuteNonQuery();
                        if (s > 0)
                        {
                            trans.Commit();
                            Jasonresult = "Successfull";

                        }
                        else
                        {
                            trans.Rollback();
                        }
                    }
                    else
                    {
                        trans.Rollback();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Jasonresult = ex.Message;
            }

            return Jasonresult;
        }


        public string ReadSection(string query)
        {
            string Jasonresult = string.Empty;
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            SqlDataReader r = new SqlCommand(query, connection).ExecuteReader();
            if (r.Read())
            {
                Jasonresult = r[0].ToString();
            }
            else
            {
                Jasonresult = "Failed";
            }
            r.Close();
            connection.Close();

            return Jasonresult;
        }


        public bool CheckUser(string username, string password)
        {
            Boolean Jasonresult = false;
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            string query = "select* from account where email =@username and password =@password and systemrole = 'Student' and active = '1' ";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("username", username);
            cmd.Parameters.AddWithValue("password", password);
            SqlDataReader r = cmd.ExecuteReader();
            if (r.Read())
            {
                Jasonresult = true;
            }
            else
            {
                Jasonresult = false;
            }
            r.Close();
            connection.Close();



            return Jasonresult;
        }

        public bool Authorise(string hash)
        {
            Boolean Jasonresult = false;
            SqlConnection connection = new SqlConnection(ConnectionString);
            connection.Open();
            LocalServices services = new LocalServices();
            string reply = services.ReadSection("select id from account where id = '" + hash + "' and Active = '1'");

            if (reply != "Failed")
            {
                if (reply == hash)
                {
                    Jasonresult = true;
                }
                else
                {
                    Jasonresult = false;
                }

            }
            else
            {
                Jasonresult = false;
            }


            return Jasonresult;

        }

        public string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Use 10000 iterations (you can increase for more security)
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000); // Compatible with all .NET versions
            byte[] hash = pbkdf2.GetBytes(32);

            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);
        }


        public  bool VerifyPassword(string enteredPassword, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(32);

            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }

            return true;
        }


        public string MailAsync(string EmailTo, string subjectinfor, string MessageBody)
        {
            string reply = "";
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(EmailTo);
                mail.From = new MailAddress(""); 
                mail.Subject = subjectinfor;
                mail.IsBodyHtml = true;
                mail.Body = MessageBody;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "";//
                smtp.Port = 587;  //587
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("", "");
                smtp.EnableSsl = true;

                // Set the timeout value to 60 seconds
                // smtp.Timeout = 60000;

                smtp.Send(mail);
                reply = "Successful";
            }
            catch (Exception ex)
            {
                reply = ex.Message + " Import Not Successful ! , Try Again !";
            }

            return reply;
        }
        public string GenerateOrderNumber()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] orderNumberChars = new char[9];

            for (int i = 0; i < orderNumberChars.Length; i++)
            {
                orderNumberChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(orderNumberChars);
        }

        public string MailAsync2(string EmailTo, string subjectinfor, string MessageBody)
        {
            string reply = "";
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(EmailTo);
                mail.From = new MailAddress("noreply@shieldguardai.com"); // noreply@educationelite.com.ng
                mail.Subject = subjectinfor;
                mail.IsBodyHtml = true;
                mail.Body = MessageBody;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.shieldguardai.com"; // 
                smtp.Port = 587;  //
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("noreply@shieldguardai.com", "@Support2020");
                smtp.EnableSsl = false;
                // Set the timeout value to 60 seconds
                // smtp.Timeout = 60000;
                smtp.Send(mail);
                reply = "Successful";
            }
            catch (Exception ex)
            {
                reply = ex.Message + " Email Not Successful ! , Try Again !";
            }

            return reply;
        }

    }

}