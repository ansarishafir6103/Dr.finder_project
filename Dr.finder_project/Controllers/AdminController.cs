using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dr.finder_project.EDM;
namespace Dr.finder_project.Controllers
{
    public class AdminController : Controller
    {

        // GET: /Admin/
        Dr_finderEntities3 dc = new Dr_finderEntities3();
        public ActionResult Index()
        {
            if (Session["adminname"] != null)
            {
                string tdate = Convert.ToString(String.Format("{0:dd-MM-yyyy}", DateTime.Now));
                var d = (from n in dc.User_s_tbl where n.User_reg_date == tdate && n.status == "Registred" select n.User_S_Id).Count();
                var h = (from n in dc.Hospital_tbl where n.Hospital_reg_date == tdate select n.Hospital_Id).Count();
                var a = (from n in dc.Appointment_tbl where n.Appointment_Current_Date == tdate select n.Appointment_Id).Count();
                var usr = (from n in dc.User_s_tbl where n.status == "Registred" select n.User_S_Id).Count();
                var hos = (from n in dc.Hospital_tbl where n.Hospital_Status == "Approved" select n.Hospital_Id).Count();
                var all_app = (from n in dc.Appointment_tbl where n.Appointment_Status == "Complete" select n.Appointment_Id).Count();
                ViewData["drcnt"] = d;
                ViewData["hcnt"] = h;
                ViewData["acnt"] = a;
                ViewData["usrcnt"] = usr;
                ViewData["hos"] = hos;
                ViewData["all_app"] = all_app;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            string AdminName = fc["Admin_Email"];
            string AdminPassword = fc["Admin_pass"];
            if (AdminName == "ansarishafir@outlook.com" && AdminPassword == "a-zA-Z0-9")
            {
                Session["adminname"] = AdminName;
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                TempData["admin"] = "invalid Email id and Password";
                return View();
            }

        }

        public ActionResult logoutAdmin()
        {
            Session.Abandon();
            return RedirectToAction("Login", "Admin");
        }


        public ActionResult HospitalView()
        {
            if (Session["adminname"] != null)
            {
                var p = (from n in dc.Hospital_tbl where n.Hospital_Status == "Approved" select n).ToList();

                ViewData["xyz"] = p;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }

        }


        public ActionResult ShowBlockList()
        {
            if (Session["adminname"] != null)
            {


                ViewData["blist"] = (from n in dc.Hospital_tbl where n.Hospital_Status == "Blocked" select n).ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }


        public ActionResult BlockHospital(int id)
        {
            //no have a view 
            var p = dc.Hospital_tbl.Find(id);
            p.Hospital_Status = "Blocked";
            dc.SaveChanges();
            return RedirectToAction("HospitalView", "Admin");

        }

        public ActionResult UnblockHospital(int id)
        {
            //no have a view
            var p = dc.Hospital_tbl.Find(id);
            p.Hospital_Status = "Approved";
            dc.SaveChanges();
            return RedirectToAction("ShowBlockList", "Admin");

        }


        public ActionResult Bolckuser(int id)
        {
            //no have a view
            var p = dc.User_s_tbl.Find(id);
            p.status = "Blocked";
            dc.SaveChanges();
            return RedirectToAction("ViewAllusers", "Admin");


        }

        public ActionResult ShowBlockListUser()
        {
            if (Session["adminname"] != null)
            {
                ViewData["blistuser"] = (from n in dc.User_s_tbl where n.status == "Blocked" select n).ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        public ActionResult UnbolckUser(int id)
        {
            //no have a view
            var p = dc.User_s_tbl.Find(id);
            p.status = "Registred";
            dc.SaveChanges();
            return RedirectToAction("ShowBlockListUser", "Admin");
        }

        public ActionResult HospitalInfo(int id)
        {
            if (Session["adminname"] != null)
            {
                Hospital_tbl htbl = dc.Hospital_tbl.Find(id);
                ViewData["hmd"] = htbl;
                ViewData["Hmf"] = (from n in dc.Facility_tbl where n.Hospital_Id == id select n).ToList();
                ViewData["dmd"] = (from n in dc.Doctor_tbl where n.Hospital_Id == id select n).ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        public ActionResult DoctorDetailsAdmin()
        {
            if (Session["adminname"] != null)
            {
                int did = Convert.ToInt16(Request.QueryString["dtid"]);
                int cid = Convert.ToInt16(Request.QueryString["cid"]);
                var q = (from n in dc.Doctor_tbl
                         from l in dc.Doctor_Type_tbl
                         where n.Doctor_Id == cid && l.Doctor_Type_Id == did
                         select new
                         {
                             n.Doctor_Id,
                             l.Doctor_Type_Id,
                             n.Doctor_Image,
                             n.Doctor_fname,
                             n.Doctor_mname,
                             n.Doctor_lname,
                             n.Doctor_Email_Id,
                             n.Doctor_Mobile_No,
                             n.Doctor_Status,
                             n.Doctor_gender,
                             n.Doctor_city,
                             n.Doctor_Address,
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
                    obj.Doctor_Address = item.Doctor_Address;
                    obj.Doctor_city = item.Doctor_city;
                    obj.Doctor_Mobile_No = item.Doctor_Mobile_No.ToString();
                    obj.Doctor_Email_Id = item.Doctor_Email_Id;
                    obj.Doctor_Id = item.Doctor_Id;
                    obj.Doctor_Type_Id = item.Doctor_Type_Id;
                    obj.Doctor_Type_Name = item.Doctor_Type_Name;
                    obj.Doctor_Experience = item.Doctor_Experience;

                    list.Add(obj);
                }
                ViewData["usdfd"] = list;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }


        public ActionResult ApproveHospital()
        {
            if (Session["adminname"] != null)
            {
                var p = (from n in dc.Hospital_tbl where n.Hospital_Status == "Pending" select n).ToList();
                ViewData["xyz"] = p;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        public ActionResult ApproveHospital_Approved(int id)
        {
            //no have a view
            var p = dc.Hospital_tbl.Find(id);
            p.Hospital_Status = "Approved";
            dc.SaveChanges();
            return RedirectToAction("ApproveHospital");
        }

        public ActionResult Hospital_Denided(int id)
        {
            //no have a view
            var d = dc.Hospital_tbl.Find(id);
            d.Hospital_Status = "Denided";
            dc.SaveChanges();
            return RedirectToAction("ApproveHospital");
        }

        public ActionResult ViewAllusers()
        {
            if (Session["adminname"] != null)
            {
                ViewData["vusers"] = (from n in dc.User_s_tbl where n.status == "Registred" select n).ToList();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        public ActionResult EdituserData(int id)
        {
            if (Session["adminname"] != null)
            {
                ViewData["usr"] = dc.User_s_tbl.Find(id);
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        [HttpPost]
        public ActionResult EdituserData(FormCollection fc)
        {
            if (Session["adminname"] != null)
            {
                int id = int.Parse(fc["uid"].ToString());
                var ht = dc.User_s_tbl.Find(id);
                ht.User_S_Address = fc["address"];
                ht.User_S_City = fc["city"];
                ht.User_S_DOB = fc["dob"];
                ht.User_S_Email_Id = fc["email"];
                ht.User_S_fName = fc["fname"];
                ht.User_S_mName = fc["mname"];
                ht.User_s_lName = fc["lname"];
                ht.User_S_Mobile_No = decimal.Parse(fc["mob"].ToString());
                ht.User_S_Pincode = decimal.Parse(fc["pin"].ToString());

                dc.SaveChanges();
                return RedirectToAction("ViewAllusers", "Admin");
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        public ActionResult ReadMoreUesrInfo(int id)
        {
            if (Session["adminname"] != null)
            {
                User_s_tbl ur = dc.User_s_tbl.Find(id);
                ViewData["UserReadMoreInfo"] = ur;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }

        public ActionResult EditHospitalProfile(int id)
        {
            if (Session["adminname"] != null)
            {
                Hospital_tbl ht = dc.Hospital_tbl.Find(id);
                ViewData["ht"] = ht;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        [HttpPost]
        public ActionResult EditHospitalProfile(FormCollection fc, HttpPostedFileBase file)
        {
            if (Session["adminname"] != null)
            {
                decimal id = decimal.Parse(fc["hid"]);
                var ht = dc.Hospital_tbl.Find(id);
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                    file.SaveAs(path);
                    ht.Hospital_Image = pic;
                    dc.SaveChanges();
                }
                ht.Hospital_Address = fc["address"];
                ht.Hospital_Mobile_No = decimal.Parse(fc["phone"]);
                ht.Hospital_Name = fc["hname"];
                ht.Hospital_Discription = fc["desc"];
                ht.Hospital_city = fc["city"];
                ht.Hospital_Email_Id = fc["email"];
                dc.SaveChanges();

                return RedirectToAction("HospitalView", "Admin");
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }
        public ActionResult ViewAllAppointment()
        {
            if (Session["adminname"] != null)
            {
                var p =
                    (
                    from ass in dc.Assign_tbl
                    from app in dc.Appointment_tbl
                    from h in dc.Hospital_tbl
                    from u in dc.User_s_tbl
                    from d in dc.Doctor_tbl
                    where app.Appointment_Status == "Complete" &&
                    ass.Appointment_Id == app.Appointment_Id &&
                    u.User_S_Id == ass.User_S_Id && h.Hospital_Id == ass.Hospital_Id
                    && d.Doctor_Id == ass.Doctor_Id
                    select new
                    {
                        u.User_s_lName,
                        u.User_S_mName,
                        u.User_S_fName,
                        d.Doctor_lname,
                        d.Doctor_mname,
                        d.Doctor_fname,
                        h.Hospital_Name,
                        app.Appointment_Current_Date,
                        app.Appointment_Date,
                        app.Appointment_Description,
                        h.Hospital_Image,
                        d.Doctor_Image

                    }
                        );
                List<AppointmentDetails> list = new List<AppointmentDetails>();
                foreach (var item in p)
                {
                    AppointmentDetails app = new AppointmentDetails();
                    app.Hospital_Name = item.Hospital_Name;
                    app.User_S_fName = item.User_S_fName;
                    app.User_s_lName = item.User_s_lName;
                    app.User_S_mName = item.User_S_mName;
                    app.Doctor_fname = item.Doctor_fname;
                    app.Doctor_lname = item.Doctor_lname;
                    app.Doctor_mname = item.Doctor_mname;
                    app.Appointment_Current_Date = item.Appointment_Current_Date;
                    app.Appointment_Date = item.Appointment_Date;
                    app.Appointment_Description = item.Appointment_Description;
                    app.Hospital_Image = item.Hospital_Image;
                    app.Doctor_Image = item.Doctor_Image;
                    list.Add(app);

                }
                ViewData["viewAllApp"] = list;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Admin");
            }
        }


        public DrDetails ViewUpdateDoctorDetails(int did, int dt)
        {//no have a view
            DrDetails dd = new DrDetails();
            var p = (from n in dc.Doctor_tbl
                     from l in dc.Doctor_Type_tbl

                     where n.Doctor_Id == did && l.Doctor_Type_Id == dt
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
                         n.Doctor_city,
                         l.Doctor_Type_Name,
                         n.Doctor_Experience,
                         n.Doctor_Id,
                         l.Doctor_Type_Id

                     });

            foreach (var item in p)
            {
                dd.Doctor_Address = item.Doctor_Address;
                dd.Doctor_city = item.Doctor_city;
                dd.Doctor_Email_Id = item.Doctor_Email_Id;
                dd.Doctor_Experience = item.Doctor_Experience;
                dd.Doctor_fname = item.Doctor_fname;
                dd.Doctor_mname = item.Doctor_mname;
                dd.Doctor_lname = item.Doctor_lname;
                dd.Doctor_Mobile_No = item.Doctor_Mobile_No.ToString();
                dd.Doctor_Type_Name = item.Doctor_Type_Name;
                dd.Doctor_gender = item.Doctor_gender;
                dd.Doctor_Image = item.Doctor_Image;
                dd.Doctor_Id = item.Doctor_Id;
                dd.Doctor_Type_Id = item.Doctor_Type_Id;
            }

            return dd;
        }


    }
    public class AppointmentDetails
    {

        public string User_s_lName { get; set; }
        public string User_S_mName { get; set; }
        public string User_S_fName { get; set; }
        public string Doctor_lname { get; set; }
        public string Doctor_mname { get; set; }
        public string Doctor_fname { get; set; }
        public string Hospital_Name { get; set; }
        public string Appointment_Current_Date { get; set; }
        public string Appointment_Date { get; set; }
        public string Appointment_Description { get; set; }
        public string Hospital_Image { get; set; }
        public string Doctor_Image { get; set; }
    }
}
