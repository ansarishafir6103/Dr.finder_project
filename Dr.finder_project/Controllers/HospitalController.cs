using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dr.finder_project.EDM;
using System.IO;
using System.Net.Mail;
namespace Dr.finder_project.Controllers
{

    public class HospitalController : Controller
    {
        Dr_finderEntities3 dc = new Dr_finderEntities3();
        //
        // GET: /Hospital/

        public ActionResult HospitalIndex()
        {
            if (Session["Hoslogin"] != null)
            {
                int hid = int.Parse(Session["Hoslogin"].ToString());
                string tdate = Convert.ToString(String.Format("{0:dd-MM-yyyy}", DateTime.Now));
                var Todayapp = (from n in dc.Appointment_tbl where n.Hospital_Id == hid && n.Appointment_Current_Date == tdate select n.Appointment_Id).Count();
                var Allapp = (from n in dc.Appointment_tbl where n.Hospital_Id == hid && n.Appointment_Status == "Complete" select n.Appointment_Id).Count();
                var todaydr = (from n in dc.Doctor_tbl where n.Hospital_Id == hid && n.Doctor_reg_date == tdate select n.Doctor_Id).Count();
                var Alldr = (from n in dc.Doctor_tbl where n.Hospital_Id == hid select n.Doctor_Id).Count();
                ViewData["Alldr"] = Alldr;
                ViewData["todaydr"] = todaydr;
                ViewData["todayapp"] = Todayapp;
                ViewData["Allapp"] = Allapp;

                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }

        }
        public ActionResult HospitalLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult HospitalLogin(FormCollection fc)
        {
            string UserName = fc["User_Name"];
            string Password = fc["Password"];

            var qry = (from n in dc.Hospital_tbl where n.Hospital_Email_Id == UserName && n.Hospital_password == Password select n).Count();
            if (qry == 1)
            {

                var pend = (from n in dc.Hospital_tbl
                            where n.Hospital_Status == "Pending" && n.Hospital_Email_Id == UserName && n.Hospital_password == Password ||
                                n.Hospital_Status == "Denided" && n.Hospital_Email_Id == UserName && n.Hospital_password == Password ||
                                n.Hospital_Status == "Approved" && n.Hospital_Email_Id == UserName && n.Hospital_password == Password ||
                                n.Hospital_Status == "Blocked" && n.Hospital_Email_Id == UserName && n.Hospital_password == Password
                            select n).FirstOrDefault();


                if (pend.Hospital_Status == "Pending")
                {
                    ViewBag.msg = "Your registration is pending";
                }
                else if (pend.Hospital_Status == "Denided")
                {
                    TempData["dn"] = "<Script>alert('Your information is fake this situation your registration is rejected by the Admin')</Script>";
                    return RedirectToAction("HospitalLogin", "Hospital");
                }
                else if (pend.Hospital_Status == "Blocked")
                {
                    TempData["dn"] = "Your Account is block";
                    return RedirectToAction("HospitalLogin", "Hospital");
                }
                else if (pend.Hospital_Status == "Approved")
                {

                    var id = pend.Hospital_Id;
                    Session["Hoslogin"] = pend.Hospital_Id;
                    Session["Hosname"] = pend.Hospital_Name;
                    return RedirectToAction("HospitalIndex", "Hospital");
                }

            }
            else
            {
                ViewBag.msg = "Invalid User Name OR Password";

            }
            return View();
        }

        public ActionResult viewRating()
        {
            if (Session["Hoslogin"] != null)
            {
                int id = int.Parse(Session["Hoslogin"].ToString());

                var p = (from r in dc.Review_tbl
                         from u in dc.User_s_tbl
                         from h in dc.Hospital_tbl
                         where r.User_S_Id == u.User_S_Id && r.Hospital_Id == h.Hospital_Id && r.Hospital_Id == id
                         select new
                         {
                             r.No_Of_Rating,
                             r.Review_Description,
                             u.User_s_lName,
                             u.User_S_fName,
                             u.User_S_mName,
                             r.Review_Date

                         }).ToList();

                List<ratings> rt = new List<ratings>();
                foreach (var item in p)
                {
                    ratings r = new ratings();
                    r.No_Of_Rating = int.Parse(item.No_Of_Rating.ToString());
                    r.Review_Date = item.Review_Date.ToString();
                    r.Review_Description = item.Review_Description;
                    r.User_s_lName = item.User_s_lName;
                    r.User_S_mName = item.User_S_mName;
                    r.User_S_fName = item.User_S_fName;
                    rt.Add(r);
                }
                ViewData["ratings"] = rt;
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("HospitalLogin", "Hospital");
        }

        public ActionResult forgetpwd()
        {

            return View();
        }
        [HttpPost]
        public ActionResult forgetpwd(FormCollection fc)
        {
            string email = fc["email"];
            decimal phone = decimal.Parse(fc["phone"].ToString());
            var q = dc.Hospital_tbl.Where(c => c.Hospital_Email_Id == email && c.Hospital_Mobile_No == phone).Count();
            if (q == 1)
            {
                var p = dc.Hospital_tbl.Where(c => c.Hospital_Email_Id == email && c.Hospital_Mobile_No == phone).FirstOrDefault();
                string pwd;

                pwd = p.Hospital_password;
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

        public ActionResult HospitalReg()
        {
            return View();
        }

        [HttpPost]
        public ActionResult HospitalReg(FormCollection fc, HttpPostedFileBase file)
        {

            string Hos_Email = fc["Hos_Email"];

            var p = (from n in dc.Hospital_tbl where n.Hospital_Email_Id == Hos_Email select n).Count();
            if (p == 0)
            {



                Hospital_tbl ht = new Hospital_tbl();
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                    file.SaveAs(path);
                    ht.Hospital_Image = pic;
                }
                string Hos_Name = fc["Hos_Name"];
                string Hos_Image = fc["pic"];
                string Hos_Mob_No = fc["Hos_Mob_No"];

                string Hos_Add = fc["Hos_Add"];
                string Hos_Des = fc["Hos_Des"];
                string Hos_Status = "Pending";
                string Hos_Password = fc["Password"];


                ht.Hospital_Name = Hos_Name;
                //string fullpath = "";

                ht.Hospital_reg_date = Convert.ToString(String.Format("{0:dd-MM-yyyy}", DateTime.Now));
                ht.Hospital_Mobile_No = decimal.Parse(Hos_Mob_No);
                ht.Hospital_Email_Id = Hos_Email;
                ht.Hospital_city = fc["city"];
                ht.Hospital_Address = Hos_Add;
                ht.Hospital_Discription = Hos_Des;
                ht.Hospital_Status = Hos_Status;
                ht.Hospital_password = Hos_Password;
                dc.Hospital_tbl.Add(ht);
                dc.SaveChanges();

                return RedirectToAction("HospitalLogin", "Hospital");
            }
            else
            {
                TempData["hemail"] = "Eneter unique Email Address";
                return View();
            }
        }

        public ActionResult HospitalUpdate()
        {
            if (Session["Hosname"] != null)
            {
                int Hospital_id = Convert.ToInt16(Session["Hoslogin"]);
                Hospital_tbl htbl = dc.Hospital_tbl.Find(Hospital_id);
                var p = (dc.Facility_tbl.Where(c => c.Hospital_Id == Hospital_id)).ToList();
                ViewData["Hprofile"] = htbl;
                ViewData["vf"] = p;
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }

        }

        public ActionResult EditProfile()
        {
            if (Session["Hoslogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }
        [HttpPost]
        public ActionResult EditProfile(FormCollection fc)
        {
            if (Session["Hoslogin"] != null)
            {
                var update = fc["update"];
                int id = int.Parse(fc["hid"]);

                var p = dc.Hospital_tbl.Find(id);

                var chg = fc["chg"];



                if (chg == "name")
                {
                    p.Hospital_Name = update;
                    dc.SaveChanges();
                }
                else if (chg == "address")
                {
                    p.Hospital_Address = update;
                    dc.SaveChanges();
                }
                else if (chg == "no")
                {
                    p.Hospital_Mobile_No = decimal.Parse(update);
                    dc.SaveChanges();
                }
                else
                {
                    ViewBag.upt = "try again";
                }
                TempData["update"] = "successfully Updated Record";
                return RedirectToAction("HospitalUpdate", "Hospital");
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }

        public ActionResult Changimg()
        {
            if (Session["Hoslogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }
        [HttpPost]
        public ActionResult Changimg(FormCollection fc, HttpPostedFileBase file)
        {
            if (Session["Hoslogin"] != null)
            {
                int hid = Convert.ToInt16(fc["hid"]);
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                    file.SaveAs(path);
                    var h = dc.Hospital_tbl.Find(hid);
                    h.Hospital_Image = pic;
                    dc.SaveChanges();

                }


                TempData["himg"] = "successfully Updated Record";
                return RedirectToAction("HospitalUpdate", "Hospital");
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }
        public ActionResult ViewFacility()
        {
            if (Session["Hoslogin"] != null)
            {
                int hosid = Convert.ToInt16(Session["Hoslogin"]);
                ViewData["vf"] = (from n in dc.Facility_tbl where n.Hospital_Id == hosid select n).ToList();
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }
        public ActionResult DoctorDetails()
        {
            if (Session["Hoslogin"] != null)
            {
                ViewData["dr"] = dc.Doctor_Type_tbl.ToList();
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }

        [HttpPost]
        public ActionResult DoctorDetails(FormCollection fc, HttpPostedFileBase file)
        {
            if (Session["Hoslogin"] != null)
            {
                string Dr_Email = fc["Dr_Email"];
                var p = (from n in dc.Doctor_tbl where n.Doctor_Email_Id == Dr_Email select n).Count();
                if (p == 0)
                {


                    Doctor_tbl dt = new Doctor_tbl();
                    if (file != null)
                    {
                        string pic = System.IO.Path.GetFileName(file.FileName);
                        string path = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                        file.SaveAs(path);
                        string Dr_Img = pic;
                        dt.Doctor_Image = Dr_Img;
                    }
                    string Hospital_Id = Session["Hoslogin"].ToString();
                    string Dr_fname = fc["Dr_fname"];
                    string Dr_mname = fc["Dr_mname"];
                    string Dr_lname = fc["Dr_lname"];
                    string Dr_Mob_No = fc["Dr_Mob_No"];
                    string Dr_Status = "Active";

                    string Password = fc["Password"];
                    string gender = fc["gender"];


                    dt.Hospital_Id = decimal.Parse(Hospital_Id);
                    dt.Doctor_fname = Dr_fname;
                    dt.Doctor_mname = Dr_mname;
                    dt.Doctor_lname = Dr_lname;
                    dt.Doctor_city = fc["city"];
                    dt.Doctor_Experience = fc["Dr_expr"];
                    dt.Doctor_gender = gender;
                    dt.Doctor_reg_date = Convert.ToString(String.Format("{0:dd-MM-yyyy}", DateTime.Now));
                    dt.Doctor_Type_Id = decimal.Parse(fc["Dr_type"]);
                    dt.Doctor_Mobile_No = decimal.Parse(Dr_Mob_No);
                    dt.Doctor_Status = Dr_Status;
                    dt.Doctor_Address = fc["Dr_add"];
                    dt.Doctor_Email_Id = Dr_Email;
                    dt.Doctor_password = Password;

                    dc.Doctor_tbl.Add(dt);
                    dc.SaveChanges();
                    TempData["drreg"] = "Registration is success full";
                    return RedirectToAction("DoctorDetails", "Hospital");
                }
                else
                {
                    TempData["dremail"] = "Enter Unique Email Address";
                    return RedirectToAction("DoctorDetails", "Hospital");
                }
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }
        public ActionResult ViewDrDetails()
        {
            if (Session["Hoslogin"] != null)
            {
                int Hospital_Id = int.Parse(Session["Hoslogin"].ToString());
                ViewData["vDetails"] = (from n in dc.Doctor_tbl where n.Hospital_Id == Hospital_Id select n).ToList();
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }

        public ActionResult DrMoreDetails()
        {
            if (Session["Hoslogin"] != null)
            {
                int did = Convert.ToInt16(Request.QueryString["dtid"]);
                int cid = Convert.ToInt16(Request.QueryString["cid"]);
                var c = (from n in dc.Certificate_tbl where n.Doctor_Id == cid select n).ToList();
                var q = (from n in dc.Doctor_tbl
                         from l in dc.Doctor_Type_tbl
                         where n.Doctor_Id == cid && l.Doctor_Type_Id == did
                         select new
                         {
                             n.Doctor_Address,
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
                    obj.Doctor_Address = item.Doctor_Address;

                    list.Add(obj);
                }
                ViewData["vcrt"] = c;
                ViewData["drmoreDetails"] = list;
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }

        public ActionResult DoctorType()
        {
            if (Session["Hoslogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }
        [HttpPost]
        public ActionResult DoctorType(FormCollection fc)
        {
            if (Session["Hoslogin"] != null)
            {
                Doctor_Type_tbl drt = new Doctor_Type_tbl();
                drt.Doctor_Type_Name = fc["Dr_type_name"];
                dc.Doctor_Type_tbl.Add(drt);
                dc.SaveChanges();
                TempData["dtype"] = "Succsess fully Submited";
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }
        public ActionResult AddFacility()
        {
            if (Session["Hoslogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }
        [HttpPost]
        public ActionResult AddFacility(FormCollection fc)
        {
            if (Session["Hoslogin"] != null)
            {
                string hosid = Session["Hoslogin"].ToString();
                Facility_tbl ft = new Facility_tbl();
                ft.Facility_Name = fc["fc_name"];
                ft.Facility_Description = fc["fc_des"];
                ft.Hospital_Id = decimal.Parse(hosid);
                dc.Facility_tbl.Add(ft);
                dc.SaveChanges();
                TempData["af"] = "your facility is added";
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }

        public ActionResult viewuApp()
        {
            if (Session["Hoslogin"] != null)
            {
                int hosid = Convert.ToInt16(Session["Hoslogin"]);
                var v = (from app in dc.Appointment_tbl
                         from u in dc.User_s_tbl
                         where app.User_S_Id == u.User_S_Id && app.Hospital_Id == hosid && app.Appointment_Status == "Pending"
                         select new
                         {
                             app.Appointment_Id,
                             app.Appointment_Current_Date,
                             app.Appointment_Date,
                             app.Appointment_Description,
                             u.User_S_mName

                         });
                //var v =dc.viewapp(1,hosid,"Pending").ToList();
                List<appDetails> list = new List<appDetails>();
                foreach (var item in v)
                {
                    appDetails ap = new appDetails();
                    ap.Appointment_Current_Date = (item.Appointment_Current_Date.ToString());
                    ap.Appointment_Date = (item.Appointment_Date.ToString());
                    ap.Appointment_Description = item.Appointment_Description;
                    ap.User_S_mName = item.User_S_mName;
                    ap.Appointment_Id = item.Appointment_Id;
                    list.Add(ap);
                }
                ViewData["vapp"] = list;
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }

        public ActionResult Approveapp(int app_id)
        {
            //no have a view
            dc.upapp(1, app_id, "Approved");
            TempData["approveapp"] = "<script>alert('Appointment is Approve');</script>";
            return RedirectToAction("viewuapp", "Hospital");
        }

        public ActionResult Denided(int app_id)
        {//no have a view
            var p = dc.Appointment_tbl.Find(app_id);
            p.Appointment_Status = "Denided";
            dc.SaveChanges();
            return RedirectToAction("viewuapp", "Hospital"); ;
        }


        public ActionResult AssignAppointment()
        {
            if (Session["Hoslogin"] != null)
            {
                int hosid = Convert.ToInt16(Session["Hoslogin"]);
                var v = (from app in dc.Appointment_tbl
                         from u in dc.User_s_tbl
                         where app.User_S_Id == u.User_S_Id && app.Hospital_Id == hosid && app.Appointment_Status == "Approved"
                         select new
                         {
                             app.Appointment_Id,
                             app.Appointment_Current_Date,
                             app.Appointment_Date,
                             app.Appointment_Description,
                             u.User_S_mName

                         });
                //var v =dc.viewapp(1,hosid,"Pending").ToList();
                List<appDetails> list = new List<appDetails>();
                foreach (var item in v)
                {
                    appDetails ap = new appDetails();
                    ap.Appointment_Current_Date = (item.Appointment_Current_Date.ToString());
                    ap.Appointment_Date = (item.Appointment_Date.ToString());
                    ap.Appointment_Description = item.Appointment_Description;
                    ap.User_S_mName = item.User_S_mName;
                    ap.Appointment_Id = item.Appointment_Id;
                    list.Add(ap);
                }
                ViewData["vappas"] = list;
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }
        public ActionResult viewAssignAppointment(int id)
        {
            if (Session["Hoslogin"] != null)
            {
                int hosid = Convert.ToInt16(Session["Hoslogin"]);
                viewapp_Result vr = dc.viewapp(2, id, null).FirstOrDefault();
                ViewData["assignview"] = vr;
                ViewData["drname"] = dc.tbldrview(hosid, "Active").ToList();
                return View();

            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }

        [HttpPost]
        public ActionResult viewAssignAppointment(FormCollection fc)
        {
            if (Session["Hoslogin"] != null)
            {
                int hosid = Convert.ToInt16(Session["Hoslogin"]);
                int aid = Convert.ToInt16(fc["apid"]);
                int uid = Convert.ToInt16(fc["uid"]);
                int drid = Convert.ToInt16(fc["drn"]);
                dc.insassign(aid, uid, hosid, drid);
                dc.SaveChanges();
                dc.upapp(1, aid, "Assign");
                dc.SaveChanges();
                TempData["asi"] = "<script>alert('Appointment is assign');</script>";
                return RedirectToAction("AssignAppointment", "Hospital");
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }

        public ActionResult DrEdit(int did, int dtid)
        {
            if (Session["Hoslogin"] != null)
            {
                AdminController ac = new AdminController();
                ViewData["dt"] = ac.ViewUpdateDoctorDetails(did, dtid);
                ViewData["dtype"] = dc.Doctor_Type_tbl.ToList();
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }

        [HttpPost]
        public ActionResult DrEdit(FormCollection fc, HttpPostedFileBase file)
        {
            if (Session["Hoslogin"] != null)
            {
                string ss = fc["t1"];
                int did = int.Parse(fc["did"]);
                int dtid = int.Parse(fc["dtid"]);
                var p = dc.Doctor_tbl.Find(did);
                var q = dc.Doctor_Type_tbl.Find(dtid);

                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                    file.SaveAs(path);
                    p.Doctor_Image = pic;
                }

                p.Doctor_Address = fc["add"];
                p.Doctor_city = fc["city"];
                p.Doctor_Email_Id = fc["email"];
                p.Doctor_Experience = fc["expr"];
                p.Doctor_fname = fc["fname"];
                p.Doctor_mname = fc["mname"];
                p.Doctor_lname = fc["lname"];
                p.Doctor_gender = fc["gndr"];
                p.Doctor_Mobile_No = decimal.Parse(fc["mob"]);
                q.Doctor_Type_Name = fc["dtype"];
                dc.SaveChanges();
                return RedirectToAction("ViewDrDetails", "Hospital");
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }

        public ActionResult Completedapp()
        {
            if (Session["Hoslogin"] != null)
            {
                int hosid = int.Parse(Session["Hoslogin"].ToString());
                var cmpapp =
                    (
                    from ass in dc.Assign_tbl
                    from app in dc.Appointment_tbl
                    from u in dc.User_s_tbl
                    from d in dc.Doctor_tbl
                    from h in dc.Hospital_tbl
                    where ass.Appointment_Id == app.Appointment_Id &&
                    ass.Doctor_Id == d.Doctor_Id &&
                    ass.User_S_Id == u.User_S_Id &&
                    ass.Hospital_Id == h.Hospital_Id &&
                    ass.Hospital_Id == hosid && app.Appointment_Status == "Complete"
                    select new
                    {
                        u.User_s_lName,
                        u.User_S_mName,
                        u.User_S_fName,
                        d.Doctor_fname,
                        d.Doctor_lname,
                        d.Doctor_mname,
                        app.Appointment_Current_Date,
                        app.Appointment_Date,
                        app.Appointment_Description,
                        d.Doctor_Image
                    }
                    );
                List<HospitalCmpDetails> list = new List<HospitalCmpDetails>();
                foreach (var item in cmpapp)
                {
                    HospitalCmpDetails obj = new HospitalCmpDetails();
                    obj.Appointment_Current_Date = item.Appointment_Current_Date;
                    obj.Appointment_Date = item.Appointment_Date;
                    obj.Appointment_Description = item.Appointment_Description;
                    obj.Doctor_mname = item.Doctor_mname;
                    obj.Doctor_fname = item.Doctor_fname;
                    obj.Doctor_lname = item.Doctor_lname;
                    obj.User_S_fNam = item.User_S_fName;
                    obj.User_s_lName = item.User_s_lName;
                    obj.User_S_mName = item.User_S_mName;
                    obj.Doctor_Image = item.Doctor_Image;
                    list.Add(obj);
                }
                ViewData["happ"] = list;
                return View();
            }
            else
            {
                return RedirectToAction("HospitalLogin", "Hospital");

            }
        }
    }






    public class appDetails
    {
        public decimal Appointment_Id { get; set; }
        public string Appointment_Current_Date { get; set; }
        public string Appointment_Date { get; set; }
        public string Appointment_Description { get; set; }
        public string User_S_mName { get; set; }
    }
    public class DrDetails
    {
        public string Doctor_Image { get; set; }
        public string Doctor_fname { get; set; }
        public string Doctor_mname { get; set; }
        public string Doctor_lname { get; set; }
        public string Doctor_Email_Id { get; set; }
        public string Doctor_Mobile_No { get; set; }
        public string Doctor_Status { get; set; }
        public string Doctor_gender { get; set; }
        public string Doctor_Type_Name { get; set; }
        public string Doctor_Experience { get; set; }
        public decimal Doctor_Id { get; set; }
        public decimal Doctor_Type_Id { get; set; }
        public string Doctor_Address { get; set; }
        public string Doctor_city { get; set; }
        public decimal Certificate_Id { get; set; }
        public string Certificate_Details { get; set; }
        public string Certificate_Image { get; set; }

    }


    public class HospitalCmpDetails
    {

        public string User_s_lName { get; set; }
        public string User_S_mName { get; set; }
        public string User_S_fNam { get; set; }
        public string Doctor_fname { get; set; }
        public string Doctor_lname { get; set; }
        public string Doctor_mname { get; set; }
        public string Appointment_Current_Date { get; set; }
        public string Appointment_Date { get; set; }
        public string Appointment_Description { get; set; }
        public string Doctor_Image { get; set; }
    }
    public class ratings
    {
        public int No_Of_Rating { get; set; }
        public string Review_Description { get; set; }
        public string User_s_lName { get; set; }
        public string Review_Date { get; set; }
        public string User_S_fName { get; set; }
        public string User_S_mName { get; set; }

    }

    public class email1
    {
        public string SendEmail(string toAddress, string subject, string body)
        {
            string result = "Message Sent Successfully..!!";
            string senderID = "doctorfinder123@gmail.com";// use sender’s email id here..
            const string senderPassword = "@doctorfinder123"; // sender password here…
            try
            {
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // smtp server address here…
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(senderID, senderPassword),
                    Timeout = 30000,
                };
                MailMessage message = new MailMessage(senderID, toAddress, subject, body);
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                result = "Error sending email.!!!";
            }
            return result;

        }

    }


}
