using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dr.finder_project.EDM;

namespace Dr.finder_project.Controllers
{
    public class UserHomeController : Controller
    {
        //
        // GET: /UserHome/
        Dr_finderEntities3 dc = new Dr_finderEntities3();


        [HttpGet]

        public ActionResult UserHome()
        {
            return View();
        }
        [HttpGet]
        public ActionResult SearchHospitalUser()
        {
            ViewData["Uview"] = (from n in dc.Hospital_tbl where n.Hospital_Status == "Approved" select n).ToList();
            return View();
        }

        [HttpPost]
        public JsonResult searchtext(string srchtext)
        {

            return Json(new { result = "Redirect", url = Url.Action("SearchHospitalUser", "UserHome", new { id = srchtext }) });

        }

        [HttpPost]
        public ActionResult Insertreview(FormCollection fc)
        {



            var aid = fc["assid"];
            var asid = decimal.Parse(aid);
            int star = int.Parse(fc["star"]);
            var p = (from n in dc.Assign_tbl where n.Assign_Id == asid select n).FirstOrDefault();
            var chek =
                (
                from n in dc.Review_tbl
                where n.Doctor_Id == p.Doctor_Id &&
                    n.Hospital_Id == p.Hospital_Id && n.User_S_Id == n.User_S_Id
                select n
                          ).Count();

            if (chek == 0)
            {


                Review_tbl rw = new Review_tbl();
                rw.Doctor_Id = p.Doctor_Id;
                rw.Hospital_Id = p.Hospital_Id;
                rw.User_S_Id = p.User_S_Id;
                rw.Review_Date = System.DateTime.Now.Date;
                rw.No_Of_Rating = star;
                rw.Review_Description = fc["desc"];
                dc.Review_tbl.Add(rw);
                dc.SaveChanges();
                return RedirectToAction("CompletedApp", "UserHome");
            }
            else
            {
                var chek1 = (from n in dc.Review_tbl
                             where n.Hospital_Id == p.Hospital_Id && n.User_S_Id == n.User_S_Id
                             select n).FirstOrDefault();
                var r = dc.Review_tbl.Find(chek1.Review_Id);
                r.Doctor_Id = p.Doctor_Id;
                r.Hospital_Id = p.Hospital_Id;
                r.User_S_Id = p.User_S_Id;
                r.Review_Date = System.DateTime.Now.Date;
                r.No_Of_Rating = star;
                r.Review_Description = fc["desc"];
                dc.SaveChanges();
                return RedirectToAction("CompletedApp", "UserHome");
            }
        }

        public ActionResult UserIndex()
        {
            if (Session["UserLogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }

        }
        public ActionResult UserLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserLogin(FormCollection fc)
        {
            string UserName = fc["uname"];
            string Password = fc["Pass"];

            var qry = (from n in dc.User_s_tbl where n.User_S_Email_Id == UserName && n.User_S_Password == Password select n).Count();
            if (qry == 1)
            {
                var s = (from n in dc.User_s_tbl where n.User_S_Email_Id == UserName && n.User_S_Password == Password select n).FirstOrDefault();
                if (s.status == "Registred")
                {

                    Session["UserLogin"] = s.User_S_Id;
                    Session["Username"] = s.User_S_mName;
                    return RedirectToAction("UserIndex", "UserHome");
                }
                else
                {
                    TempData["wrong"] = "You are blocked";
                    return RedirectToAction("UserLogin", "UserHome");
                }
            }
            else
            {
                TempData["wrong"] = "Invalid Username and password";
                return View();
            }

        }

        public ActionResult UserLogout()
        {
            Session.Abandon();
            return RedirectToAction("UserLogin", "UserHome");
        }

        public ActionResult UserReg()
        {

            return View();
        }

        [HttpPost]
        public ActionResult UserReg(FormCollection fc)
        {

            string email = fc["email"];

            var p = (from n in dc.User_s_tbl where n.User_S_Email_Id == email select n).Count();
            if (p == 0)
            {

                string pass = fc["pass"];
                string fname = fc["fname"];
                string mname = fc["mname"];
                string lname = fc["lname"];
                string cpass = fc["c_pass"];
                string mob = fc["mob"];
                string gender = fc["gender"];
                string add = fc["add"];
                string pin = fc["pin"];
                User_s_tbl ur = new User_s_tbl();
                ur.User_S_fName = fname;
                ur.User_S_mName = mname;
                ur.User_s_lName = lname;
                ur.User_S_Password = pass;
                ur.User_S_Email_Id = email;
                ur.User_S_Mobile_No = decimal.Parse(mob);
                ur.User_S_Gender = gender;
                ur.User_reg_date = Convert.ToString(String.Format("{0:dd-MM-yyyy}", DateTime.Now));
                ur.User_S_City = fc["city"];
                ur.status = "Registred";
                ur.User_S_DOB = Convert.ToString(String.Format("{0:dd-MM-yyyy}", fc["User_S_DOB"]));
                ur.User_S_Address = add;
                ur.User_S_Pincode = int.Parse(pin);
                dc.User_s_tbl.Add(ur);
                dc.SaveChanges();
                TempData["ureg"] = "You are Successful Registred";
                return RedirectToAction("UserLogin", "UserHome");
            }
            else
            {
                TempData["eid"] = "Enter Unique Email Address";
                return View();
            }
        }

        public ActionResult DoctorLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DoctorLogin(FormCollection fc)
        {

            string UserName = fc["UserName"];
            string Password = fc["Password"];
            var qry = (from n in dc.Doctor_tbl where n.Doctor_Email_Id == UserName && n.Doctor_password == Password select n).Count();
            if (qry == 1)
            {
                var q = (from n in dc.Doctor_tbl where n.Doctor_Email_Id == UserName && n.Doctor_password == Password select n).FirstOrDefault();
                Session["drlogin"] = q.Doctor_mname;
                Session["drid"] = q.Doctor_Id;
                return RedirectToAction("DoctorIndex", "Doctor");
            }
            else
            {
                TempData["wrongd"] = "Invalid Username and password";
                return RedirectToAction("DoctorLogin", "UserHome");
            }

        }

        public ActionResult Drlogut()
        {
            Session.Abandon();
            return RedirectToAction("DoctorLogin", "UserHome");
        }

        public ActionResult UviewHospital()
        {
            if (Session["UserLogin"] != null)
            {
                ViewData["Uview"] = (from n in dc.Hospital_tbl where n.Hospital_Status == "Approved" select n).ToList();
                return View();
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }

        }
        public ActionResult Umoredetails(int id)
        {
            if (Session["UserLogin"] != null)
            {
                Hospital_tbl htbl = dc.Hospital_tbl.Find(id);
                ViewData["hmd"] = htbl;
                ViewData["Hmf"] = (from n in dc.Facility_tbl where n.Hospital_Id == id select n).ToList();
                ViewData["dmd"] = (from n in dc.Doctor_tbl where n.Hospital_Id == id select n).ToList();
                return View();
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }
        }
        public ActionResult UShowDoctorFullDetails()
        {
            if (Session["UserLogin"] != null)
            {
                int did = Convert.ToInt16(Request.QueryString["dtid"]);
                int cid = Convert.ToInt16(Request.QueryString["cid"]);
                var q = (from n in dc.Doctor_tbl
                         from l in dc.Doctor_Type_tbl
                         where n.Doctor_Id == cid && l.Doctor_Type_Id == did
                         select new
                         {

                             n.Doctor_Image,
                             n.Doctor_fname,
                             n.Doctor_mname,
                             n.Doctor_lname,
                             n.Doctor_Email_Id,
                             n.Doctor_Mobile_No,
                             n.Doctor_Status,
                             n.Doctor_gender,
                             l.Doctor_Type_Name,
                             n.Doctor_Experience
                         });
                List<DrDetails> list = new List<DrDetails>();
                foreach (var item in q)
                {
                    DrDetails obj = new DrDetails();
                    obj.Doctor_fname = item.Doctor_fname;
                    obj.Doctor_mname = item.Doctor_mname;
                    obj.Doctor_lname = item.Doctor_lname;
                    obj.Doctor_Image = item.Doctor_Image;
                    obj.Doctor_gender = item.Doctor_gender;
                    obj.Doctor_Status = item.Doctor_Status;
                    obj.Doctor_Mobile_No = item.Doctor_Mobile_No.ToString();
                    obj.Doctor_Email_Id = item.Doctor_Email_Id;
                    obj.Doctor_Type_Name = item.Doctor_Type_Name;
                    obj.Doctor_Experience = item.Doctor_Experience;

                    list.Add(obj);
                }
                ViewData["usdfd"] = list;

                return View();
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }
        }

        public ActionResult BookApp()
        {
            if (Session["UserLogin"] != null)
            {

                ViewData["vhos"] = (from n in dc.Hospital_tbl where n.Hospital_Status == "Approved" select n).ToList();
                return View();
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }
        }
        [HttpPost]
        public ActionResult BookApp(FormCollection fc)
        {
            if (Session["UserLogin"] != null)
            {
                string app_desc = fc["app_desc"];
                string curr_date = Convert.ToString(String.Format("{0:dd-MM-yyyy}", DateTime.Now));
                string ap_dt = Convert.ToString(String.Format("{0:dd-MM-yyyy}", fc["Appointment_Date"].ToString()));
                string status = "Pending";
                int hos_id = Convert.ToInt16(fc["hos_id"]);
                int u_id = Convert.ToInt16(Session["UserLogin"]);
                dc.Book_app(ap_dt, curr_date, hos_id, u_id, app_desc, status);
                TempData["bookapp"] = "<script>alert('Your Appointment is submited');</script>";
                return RedirectToAction("BookApp", "UserHome");
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }
        }
        public ActionResult viewprofile()
        {
            if (Session["UserLogin"] != null)
            {
                int uid = Convert.ToInt16(Session["UserLogin"]);
                User_s_tbl utbl = (from n in dc.User_s_tbl where n.User_S_Id == uid select n).FirstOrDefault();
                ViewData["upro"] = utbl;
                return View();
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }
        }

        public ActionResult search()
        {

            if (Session["UserLogin"] != null)
            {
                ViewData["Uview_a"] = (from n in dc.Hospital_tbl where n.Hospital_Status == "Approved" select n).ToList();

                return View();
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }
        }

        public ActionResult SearchBookApp(int id2)
        {
            if (Session["UserLogin"] != null)
            {
                ViewData["sba"] = dc.Hospital_tbl.Find(id2);
                return View();
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }
        }

        [HttpPost]
        public ActionResult SearchBookApp(FormCollection fc)
        {
            if (Session["UserLogin"] != null)
            {

                string app_desc = fc["app_desc"];
                string curr_date = Convert.ToString(String.Format("{0:dd-MM-yyyy}", DateTime.Now));
                string ap_dt = Convert.ToString(String.Format("{0:dd-MM-yyyy}", fc["Appointment_Date"].ToString()));
                string status = "Pending";
                int hos_id = Convert.ToInt16(fc["hos_id"]);
                int u_id = Convert.ToInt16(Session["UserLogin"]);
                dc.Book_app(ap_dt, curr_date, hos_id, u_id, app_desc, status);

                return RedirectToAction("Search", "UserHome");
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }
        }

        public ActionResult ReadMoreHospitals(int id)
        {
            Hospital_tbl htbl = dc.Hospital_tbl.Find(id);
            ViewData["hmd"] = htbl;
            ViewData["Hmf"] = (from n in dc.Facility_tbl where n.Hospital_Id == id select n).ToList();
            return View();
        }
        public ActionResult Appstatus()
        {
            if (Session["UserLogin"] != null)
            {
                int usid = Convert.ToInt16(Session["UserLogin"]);
                var p = dc.app_chk_st(usid).ToList();
                ViewData["apst"] = p;
                return View();
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }
        }

        public ActionResult confirmapp()
        {
            if (Session["UserLogin"] != null)
            {
                int usid = Convert.ToInt16(Session["UserLogin"]);
                var q = (from dr in dc.Doctor_tbl
                         from ass in dc.Assign_tbl
                         from u in dc.User_s_tbl
                         from h in dc.Hospital_tbl
                         from app in dc.Appointment_tbl
                         where dr.Doctor_Id == ass.Doctor_Id && ass.User_S_Id == usid && ass.User_S_Id == u.User_S_Id && u.User_S_Id == usid && ass.Hospital_Id == h.Hospital_Id && app.Appointment_Id == ass.Appointment_Id && app.Appointment_Status == "Assign"
                         select new
                         {
                             dr.Doctor_mname,
                             u.User_S_fName,
                             h.Hospital_Name,
                             app.Appointment_Date,
                             app.Appointment_Current_Date,
                             app.Appointment_Description,
                             app.Appointment_Status
                         }).ToList();

                List<ass_record> li = new List<ass_record>();


                foreach (var item in q)
                {
                    ass_record obj = new ass_record();
                    obj.dr_name = item.Doctor_mname;
                    obj.User_S_fName = item.User_S_fName;
                    obj.Hospital_Name = item.Hospital_Name;
                    obj.Appointment_Date = (item.Appointment_Date).ToString();
                    obj.Appointment_Current_Date = (item.Appointment_Current_Date).ToString();
                    obj.Appointment_Description = item.Appointment_Description;
                    obj.Appointment_Status = item.Appointment_Status;
                    li.Add(obj);

                }

                ViewData["apst_ass"] = li;

                return View();
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }
        }


        public ActionResult CompletedApp()
        {
            if (Session["UserLogin"] != null)
            {
                int usid = Convert.ToInt16(Session["UserLogin"]);
                var q = (from dr in dc.Doctor_tbl
                         from ass in dc.Assign_tbl
                         from u in dc.User_s_tbl
                         from h in dc.Hospital_tbl
                         from app in dc.Appointment_tbl
                         where dr.Doctor_Id == ass.Doctor_Id &&
                         ass.User_S_Id == usid && ass.User_S_Id == u.User_S_Id && u.User_S_Id == usid && ass.Hospital_Id == h.Hospital_Id && app.Appointment_Id == ass.Appointment_Id && app.Appointment_Status == "Complete"
                         select new
                         {
                             ass.Hospital_Id,
                             ass.User_S_Id,
                             ass.Doctor_Id,
                             ass.Assign_Id,
                             dr.Doctor_mname,
                             u.User_S_fName,
                             h.Hospital_Name,
                             app.Appointment_Date,
                             app.Appointment_Current_Date,
                             app.Appointment_Description,
                             app.Appointment_Status
                         }).ToList();

                List<ass_record> li = new List<ass_record>();


                foreach (var item in q)
                {
                    ass_record obj = new ass_record();
                    obj.Assign_Id = int.Parse(item.Assign_Id.ToString());
                    obj.Doctor_Id = int.Parse(item.Doctor_Id.ToString());
                    obj.Hospital_Id = int.Parse(item.Hospital_Id.ToString());
                    obj.User_S_Id = int.Parse(item.User_S_Id.ToString());
                    obj.dr_name = item.Doctor_mname;
                    obj.User_S_fName = item.User_S_fName;
                    obj.Hospital_Name = item.Hospital_Name;
                    obj.Appointment_Date = (item.Appointment_Date).ToString();
                    obj.Appointment_Current_Date = (item.Appointment_Current_Date).ToString();
                    obj.Appointment_Description = item.Appointment_Description;
                    obj.Appointment_Status = item.Appointment_Status;
                    li.Add(obj);

                }

                ViewData["apst_cmp"] = li;

                return View();
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }
        }

        public ActionResult UpdateUserProfile()
        {
            if (Session["UserLogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }
        }
        [HttpPost]
        public ActionResult UpdateUserProfile(FormCollection fc)
        {
            if (Session["UserLogin"] != null)
            {
                string chng = fc["chvar"];
                int id = Convert.ToInt16(fc["uid"]);
                string updte = fc["updt"];

                var uid = dc.User_s_tbl.Find(id);

                if (chng == "gnd")
                {
                    uid.User_S_Gender = updte;

                }
                else if (chng == "address")
                {
                    uid.User_S_Address = updte;

                }
                else if (chng == "phon")
                {
                    uid.User_S_Mobile_No = decimal.Parse(updte);
                }
                else if (chng == "pin")
                {
                    uid.User_S_Pincode = decimal.Parse(updte);
                }
                else if (chng == "city")
                {
                    uid.User_S_City = updte;
                }
                dc.SaveChanges();
                return RedirectToAction("viewprofile", "UserHome");
            }
            else
            {
                return RedirectToAction("UserLogin", "UserHome");
            }
        }

        public ActionResult ForgetPassUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassUser(FormCollection fc)
        {
            string email = fc["email"];
            decimal phone = decimal.Parse(fc["phone"].ToString());
            var q = dc.User_s_tbl.Where(c => c.User_S_Email_Id == email && c.User_S_Mobile_No == phone).Count();
            if (q == 1)
            {
                var p = dc.User_s_tbl.Where(c => c.User_S_Email_Id == email && c.User_S_Mobile_No == phone).FirstOrDefault();
                string pwd;

                pwd = p.User_S_Password;
                string data = "";
                string sub = "";
                data = "password: " + pwd + "\r\nEmail: " + email + "\r\nMessage: ";

                email1 e = new email1();
                e.SendEmail(email, sub, data);



                TempData["key1"] = "<script>alert('Password Sent to your E-mail')</script>";
                return View();
            }
            else
            {
                TempData["key1"] = "<script>alert('Invalid....')</script>";
                return View();
            }
        }
    }

    public class ass_record
    {
        public int Hospital_Id { get; set; }
        public int User_S_Id { get; set; }
        public int Doctor_Id { get; set; }
        public int Assign_Id { get; set; }
        public string dr_name { get; set; }
        public string User_S_fName { get; set; }
        public string Hospital_Name { get; set; }
        public string Appointment_Date { get; set; }
        public string Appointment_Current_Date { get; set; }
        public string Appointment_Description { get; set; }
        public string Appointment_Status { get; set; }

    }

}
