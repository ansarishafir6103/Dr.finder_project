using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dr.finder_project.EDM;
using System.IO;

namespace Dr.finder_project.Controllers
{
    public class DoctorController : Controller
    {
        //
        // GET: /Doctor/
        Dr_finderEntities3 dc = new Dr_finderEntities3();
        public ActionResult DoctorIndex()
        {
            if (Session["drlogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("DoctorLogin", "UserHome");
            }

        }



        public ActionResult viewProfile()
        {
            if (Session["drlogin"] != null)
            {
                int id = int.Parse(Session["drid"].ToString());

                var q = (from m in dc.Doctor_tbl
                         from n in dc.Doctor_Type_tbl
                         where m.Doctor_Id == id && m.Doctor_Type_Id == n.Doctor_Type_Id
                         select new
                             {
                                 m.Doctor_Id,
                                 n.Doctor_Type_Id,
                                 m.Doctor_fname,
                                 m.Doctor_mname,
                                 m.Doctor_lname,
                                 m.Doctor_Email_Id,
                                 m.Doctor_gender,
                                 m.Doctor_Mobile_No,
                                 m.Doctor_Address,
                                 m.Doctor_Status,
                                 m.Doctor_Image,
                                 n.Doctor_Type_Name,
                                 m.Doctor_Experience
                             });
                List<DrDetails> list = new List<DrDetails>();

                foreach (var item in q)
                {
                    DrDetails obj = new DrDetails();
                    obj.Doctor_Id = item.Doctor_Id;
                    obj.Doctor_Type_Id = item.Doctor_Type_Id;
                    obj.Doctor_fname = item.Doctor_fname;
                    obj.Doctor_mname = item.Doctor_mname;
                    obj.Doctor_lname = item.Doctor_mname;
                    obj.Doctor_gender = item.Doctor_gender;
                    obj.Doctor_Email_Id = item.Doctor_Email_Id;
                    obj.Doctor_Mobile_No = item.Doctor_Mobile_No.ToString();
                    obj.Doctor_Status = item.Doctor_Status;
                    obj.Doctor_Type_Name = item.Doctor_Type_Name;
                    obj.Doctor_Address = item.Doctor_Address;
                    obj.Doctor_Experience = item.Doctor_Experience;
                    obj.Doctor_Image = item.Doctor_Image;
                    list.Add(obj);
                }


                var prf = (dc.Certificate_tbl.Where(c => c.Doctor_Id == id)).ToList();

                ViewData["vp"] = list;
                ViewData["prf"] = prf;
                return View();
            }
            else
            {
                return RedirectToAction("DoctorLogin", "UserHome");
            }
        }
        public ActionResult viewDoctorProfileImage()
        {
            if (Session["drlogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("DoctorLogin", "UserHome");
            }
        }
        [HttpPost]
        public ActionResult viewDoctorProfileImage(FormCollection fc, HttpPostedFileBase file)
        {
            if (Session["drlogin"] != null)
            {
                int id = int.Parse(fc["id"].ToString());
                if (fc["cng"] == "image")
                {
                    if (file != null)
                    {


                        string pic = System.IO.Path.GetFileName(file.FileName);
                        string path = System.IO.Path.Combine(Server.MapPath("~/Images"), pic);
                        file.SaveAs(path);
                        var p = dc.Doctor_tbl.Find(id);
                        p.Doctor_Image = pic;
                        dc.SaveChanges();
                    }
                }
                return RedirectToAction("viewProfile", "Doctor");
            }
            else
            {
                return RedirectToAction("DoctorLogin", "UserHome");
            }
        }

        public ActionResult ViewDoctorProfileForm()
        {
            if (Session["drlogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("DoctorLogin", "UserHome");
            }
        }

        [HttpPost]
        public ActionResult ViewDoctorProfileForm(FormCollection fc)
        {
            if (Session["drlogin"] != null)
            {
                int id = int.Parse(fc["id"].ToString());
                var p = dc.Doctor_tbl.Find(id);
                if (fc["cng"] == "fname")
                {

                    p.Doctor_fname = fc["updt"];
                }
                else if (fc["cng"] == "mname")
                {
                    p.Doctor_mname = fc["updt"];
                }
                else if (fc["cng"] == "lname")
                {
                    p.Doctor_lname = fc["updt"];
                }
                else if (fc["cng"] == "mob")
                {
                    p.Doctor_Mobile_No = decimal.Parse(fc["updt"]);
                }
                else if (fc["cng"] == "status")
                {
                    p.Doctor_Status = fc["updt"];
                }
                else if (fc["cng"] == "gender")
                {
                    p.Doctor_gender = fc["updt"];
                }
                else if (fc["cng"] == "exp")
                {
                    p.Doctor_Experience = fc["updt"];
                }
                else if (fc["cng"] == "typename")
                {
                    var q = dc.Doctor_Type_tbl.Find(id);
                    q.Doctor_Type_Name = fc["updt"];
                }
                else if (fc["cng"] == "draddres")
                {
                    p.Doctor_Address = fc["updt"];
                }
                else
                {
                    TempData["in"] = "Invalid input";
                }
                dc.SaveChanges();
                TempData["msg"] = "successfully Updated Record";
                return RedirectToAction("viewProfile", "Doctor");
            }
            else
            {
                return RedirectToAction("DoctorLogin", "UserHome");
            }
        }

        public ActionResult AddCertificate()
        {
            if (Session["drlogin"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("DoctorLogin", "UserHome");
            }
        }
        [HttpPost]
        public ActionResult AddCertificate(FormCollection fc, HttpPostedFileBase c_image)
        {
            if (Session["drlogin"] != null)
            {
                int id = int.Parse(Session["drid"].ToString());
                Certificate_tbl crt = new Certificate_tbl();
                if (c_image != null)
                {
                    string filename = System.IO.Path.GetFileName(c_image.FileName);
                    string fullpath = System.IO.Path.Combine(Server.MapPath("~/Images"), filename);
                    c_image.SaveAs(fullpath);
                    crt.Certificate_Image = filename;

                }
                crt.Doctor_Id = id;
                crt.Certificate_Details = fc["c_details"];
                dc.Certificate_tbl.Add(crt);
                dc.SaveChanges();
                TempData["drcrt"] = "Your Details Are Submited";
                return View();
            }
            else
            {
                return RedirectToAction("DoctorLogin", "UserHome");
            }
        }
        public ActionResult viewdayapp()
        {
            if (Session["drlogin"] != null)
            {
                int id = Convert.ToInt16(Session["drid"]);
                var p = dc.viewdrasapp(id, "Assign").ToList();
                ViewData["asng"] = p;
                return View();
            }
            else
            {
                return RedirectToAction("DoctorLogin", "UserHome");
            }
        }
        public ActionResult completedayapp(int asid)
        {
            //dc.upapp(1,asid, "Complete");
            var p = dc.Appointment_tbl.Where(c => c.Appointment_Id == asid).FirstOrDefault();
            p.Appointment_Status = "Complete";
            dc.SaveChanges();
            return RedirectToAction("viewdayapp", "Doctor");
        }
        public ActionResult CompletedAppointment()
        {
            if (Session["drlogin"] != null)
            {
                int id = int.Parse(Session["drid"].ToString());
                var p = from a in dc.Appointment_tbl
                        from ass in dc.Assign_tbl
                        from u in dc.User_s_tbl
                        where a.Appointment_Id == ass.Appointment_Id && ass.Doctor_Id == id && u.User_S_Id == ass.User_S_Id && a.Appointment_Status == "Complete"
                        select new
                 {
                     a.Appointment_Current_Date,
                     a.Appointment_Date,
                     a.Appointment_Description,
                     u.User_S_mName

                 };
                List<complete> lst = new List<complete>();
                foreach (var item in p)
                {
                    complete c = new complete();
                    c.adate = item.Appointment_Date.ToString();
                    c.cdate = item.Appointment_Current_Date.ToString();
                    c.desc = item.Appointment_Description;
                    c.User_S_mName = item.User_S_mName;

                    lst.Add(c);
                }

                ViewData["capp"] = lst;
                return View();
            }
            else
            {
                return RedirectToAction("DoctorLogin", "UserHome");
            }
        }

        public ActionResult ForgetPassDoctor()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ForgetPassDoctor(FormCollection fc)
        {

            string email = fc["email"];
            decimal phone = decimal.Parse(fc["phone"].ToString());
            var q = dc.Doctor_tbl.Where(c => c.Doctor_Email_Id == email && c.Doctor_Mobile_No == phone).Count();
            if (q == 1)
            {
                var p = dc.Doctor_tbl.Where(c => c.Doctor_Email_Id == email && c.Doctor_Mobile_No == phone).FirstOrDefault();
                string pwd;

                pwd = p.Doctor_password;
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

    public class complete
    {
        public string cdate { get; set; }
        public string adate { get; set; }
        public string desc { get; set; }
        public string User_S_mName { get; set; }
    }
}
