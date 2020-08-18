using Practical_Test.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace Practical_Test.Controllers
{
    public class RegisterController : Controller
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader dr;
        SqlDataAdapter da;
        DataSet ds;

        public void Connection()
        {
            con = new SqlConnection(ConfigurationManager.AppSettings["connString"].ToString());
        }

        public ActionResult Index()
        {
            List<RegisterModel> objList = new List<RegisterModel>();
            try
            {
                Connection();
                con.Open();
                cmd = new SqlCommand("Usp_GetRegistrationDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    RegisterModel obj = new RegisterModel();
                    obj.ID = Convert.ToInt32(dr["ID"].ToString());
                    obj.Name = dr["Name"].ToString();
                    obj.Gender = dr["Gender"].ToString();
                    obj.Qualification = dr["Qualification"].ToString();
                    obj.Age = Convert.ToInt32(dr["Age"].ToString());
                    obj.CityName = dr["CityName"].ToString();
                    obj.StateName = dr["StateName"].ToString();
                    obj.EmailID = dr["EmailID"].ToString();
                    obj.ImageName = dr["ImageName"].ToString();
                    obj.ImagePath = dr["ImagePath"].ToString();
                    objList.Add(obj);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message.ToString() + "')</script>");
                return View();
            }
            return View(objList);
        }

        public ActionResult Delete(int ID)
        {
            try
            {
                Connection();
                con.Open();
                cmd = new SqlCommand("Usp_DeleteRegistrationDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", ID);
                int res = cmd.ExecuteNonQuery();
                if (res > 0)
                {
                    Response.Write("<script>alert('Delete Success')</script>");
                }
                else
                {
                    Response.Write("<script>alert('Delete Fail')</script>");
                }
                con.Close();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.Message.ToString() + "')</script>");
                return View();
            }
            return RedirectToAction("Index");
        }

        public string CreateXML(Object objModel)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(objModel.GetType());
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, objModel);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
        }

        public static string XmlSerializeToString(object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
        }

        public void BindState()
        {
            DataSet ds = new DataSet();
            ds = GetDropDownDetails("Select * from Tbl_StateMaster");
            List<SelectListItem> StateList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                StateList.Add(new SelectListItem { Text = dr["StateName"].ToString(), Value = dr["StateID"].ToString() });
            }
            ViewBag.StateList = StateList;
        }
        public DataSet GetDropDownDetails(string Query)
        {
            Connection();
            con.Open();
            SqlCommand cmd = new SqlCommand(Query, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }
        public JsonResult BindCity(int StateID)
        {
            DataSet ds = new DataSet();
            ds = GetDropDownDetails("Select * from Tbl_CityMaster where StateID=" + StateID);
            List<SelectListItem> CityList = new List<SelectListItem>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CityList.Add(new SelectListItem { Text = dr["CityName"].ToString(), Value = dr["CityID"].ToString() });
            }
            return Json(CityList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Register()
        {
            BindState();
            ViewBag.CityList = new List<SelectListItem>();
            return View("Register");
        }
        [HttpPost]
        public ActionResult Register(RegisterModel objModel)
        {
            try
            {
                string ImageName = "", ImagePath = "";
                if (ModelState.IsValid)
                {
                    Connection();
                    con.Open();
                    if (objModel.ImageFile != null && objModel.ImageFile.ContentLength > 0)
                    {
                        ImageName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "_" + Path.GetFileName(objModel.ImageFile.FileName);
                        ImagePath = "/UploadedImages/" + ImageName;
                        objModel.ImageFile.SaveAs(Server.MapPath(ImagePath));
                    }
                    objModel.ActionType = "Add";
                    objModel.ImageName = ImageName;
                    objModel.ImagePath = ImagePath;
                    objModel.ImageFile = null;
                    SqlCommand cmd = new SqlCommand("Usp_AddEditRegisterDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    string stXML = CreateXML(objModel);
                    // string stXML = XmlSerializeToString(objModel);
                    cmd.Parameters.AddWithValue("@XML", stXML);
                    cmd.ExecuteNonQuery();
                }
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                Response.Write("<Script>alert('" + ex.Message.ToString() + "')</script>");
            }
            finally
            {
                con.Close();
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int ID)
        {
            BindState();
            ViewBag.CityList = new List<SelectListItem>();
            RegisterModel objModel = new RegisterModel();
            try
            {
                Connection();
                con.Open();
                SqlCommand cmd = new SqlCommand("Usp_GetRegistrationDetailsByID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", ID);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    objModel.ID = Convert.ToInt32(dr["ID"]);
                    objModel.Name = dr["Name"].ToString();
                    objModel.Gender = dr["Gender"].ToString();
                    objModel.Qualification = dr["Qualification"].ToString();
                    objModel.Age = Convert.ToInt32(dr["Age"]);
                    objModel.StateID = Convert.ToInt32(dr["StateID"]);
                    objModel.CityID = Convert.ToInt32(dr["CityID"]);
                    objModel.EmailID = dr["EmailID"].ToString();
                    objModel.Pass = dr["Pass"].ToString();
                    objModel.ImageName = dr["ImageName"].ToString();
                }
            }
            catch (Exception ex)
            {
                Response.Write("<Script>alert('" + ex.Message.ToString() + "')</script>");
            }
            finally
            {
                con.Close();
            }
            return View(objModel);
        }
        [HttpPost]
        public ActionResult Edit(RegisterModel objModel)
        {
            try
            {
                string ImageName = "", ImagePath = "";
                if (ModelState.IsValid)
                {

                    Connection();
                    con.Open();
                    if (objModel.ImageFile != null && objModel.ImageFile.ContentLength > 0)
                    {
                        ImageName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "_" + Path.GetFileName(objModel.ImageFile.FileName);
                        ImagePath = "/UploadedImages/" + ImageName;
                        objModel.ImageFile.SaveAs(Server.MapPath(ImagePath));
                    }
                    objModel.ActionType = "Edit";
                    objModel.ImageName = ImageName;
                    objModel.ImagePath = ImagePath;
                    objModel.ImageFile = null;
                    SqlCommand cmd = new SqlCommand("Usp_AddEditRegisterDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    string stXML = CreateXML(objModel);
                    // string stXML = XmlSerializeToString(objModel);
                    cmd.Parameters.AddWithValue("@XML", stXML);
                    cmd.ExecuteNonQuery();
                }
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                Response.Write("<Script>alert('" + ex.Message.ToString() + "')</script>");
            }
            finally
            {
                con.Close();
            }
            return RedirectToAction("Index");
        }
    }
}