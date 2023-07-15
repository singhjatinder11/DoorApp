using Microsoft.AspNet.Identity.Owin;
using MordenDoors.Database;
using MordenDoors.Models;
using MordenDoors.Repository;
using MordenDoors.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MordenDoors.Controllers
{

    public class AdminController : Controller
    {
        private readonly MordenDoorsEntities _context = new MordenDoorsEntities();
        AdminRepository _adminRepository;
        UserRepository _userRepository;
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private string ZohoClientId = ConfigurationManager.AppSettings["ZohoClientId"].ToString();
        private string ZohoClientSecret = ConfigurationManager.AppSettings["ZohoClientSecret"].ToString();
        private string ZohoRedirectUri = ConfigurationManager.AppSettings["ZohoRedirectUri"].ToString();
        private string ZohoAuthorizeURL = ConfigurationManager.AppSettings["ZohoAuthorizeURL"].ToString();
        private string ZohoTokenURL = ConfigurationManager.AppSettings["ZohoTokenURL"].ToString();
        private string ZohoScope = ConfigurationManager.AppSettings["ZohoScope"].ToString();
        private string ZohoCustomersURL = ConfigurationManager.AppSettings["ZohoCustomersURL"].ToString();
        private string ZohoItemURL = ConfigurationManager.AppSettings["ZohoItemsURL"].ToString();
        private string ZohoInvoiceURL = ConfigurationManager.AppSettings["ZohoInvoiceURL"].ToString();
        public AdminController()
        {
            _adminRepository = new AdminRepository();
            _userRepository = new UserRepository();
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult EmployeePortal()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditEmployee(string email)
        {
            var result = _adminRepository.EmployeeDetail(email);
            return View(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditEmployee(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var res = _context.AspNetUsers.Where(x => x.Email == model.Email).FirstOrDefault();
                var oldpwd = res.PasswordHash;
                    //_context.AspNetUsers.Where(x => x.Email == model.Email).Select(x => x.PasswordHash).FirstOrDefault();
                var unselectedSkills = _context.EmployeeSkills.Where(s => !model.UserSkill.Contains(s.SkillId) && s.UserId == res.Id);
                _context.EmployeeSkills.RemoveRange(unselectedSkills);
                foreach (int skillId in model.UserSkill)
                {
                    var empSkills = _context.EmployeeSkills.Where(s => s.SkillId == skillId && s.UserId == res.Id).FirstOrDefault();
                    if (empSkills == null)
                        _context.EmployeeSkills.Add(new EmployeeSkills() { SkillId = skillId, UserId = res.Id });
                }
                _context.SaveChanges();
                int result = _adminRepository.UpdateUser(model);
                if (result > 0)
                    TempData["UpdateUser"] = result;
                else
                    TempData["UpdateUser"] = 0;
                return RedirectToAction("UsersWithRoles", "Manage");
            }
            if (!string.IsNullOrEmpty(model.Email))
                return View(_adminRepository.EmployeeDetail(model.Email));
            else 
                return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RemoveEmployee(string email)
        {
            var result = _adminRepository.RemoveUser(email);
            if (result > 0)
                TempData["RemoveUser"] = result;
            else
                TempData["RemoveUser"] = 0;
            return RedirectToAction("UsersWithRoles", "Manage");
        }
        [AllowAnonymous]
        public ActionResult Edit()
        {
            var result = _adminRepository.EmployeeDetail(User.Identity.Name);
            return View(result);
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult Edit(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                int result = _adminRepository.UpdateUser(model);
                if (result > 0)
                    TempData["UpdateUser"] = result;
                else
                    TempData["UpdateUser"] = 0;
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Detail(string email)
        {
            var result = _adminRepository.EmployeeDetail(email);
            return View(result);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult WorkShopStatus(string status)
        {
            return View(_adminRepository.WorkshopStatus(status));
        }



        [Authorize(Roles = "Admin")]
        public ActionResult ZohoBooks()
        {
            string connectUrl = string.Format("{0}?response_type=code&access_type=offline&prompt=consent&client_id={1}&redirect_uri={2}&scope={3}", ZohoAuthorizeURL, ZohoClientId, ZohoRedirectUri, ZohoScope); //Acumatica
            Response.Redirect(connectUrl);
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ZohoCallback(string code)
        {
            try
            {
                StringBuilder body = new StringBuilder();
                body.Append("grant_type=authorization_code&");
                body.Append("code=" + code + "&");
                body.Append("redirect_uri=" + Uri.EscapeDataString(ZohoRedirectUri) + "&");
                body.Append("client_id=" + ZohoClientId + "&");
                body.Append("client_secret=" + ZohoClientSecret + "&");
                var header = Convert.ToBase64String(new ASCIIEncoding().GetBytes(ZohoClientId + ":" + ZohoClientSecret));
                WebRequest req = WebRequest.Create(ZohoTokenURL);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                byte[] data = Encoding.ASCII.GetBytes(body.ToString());
                req.ContentLength = (long)data.Length;
                req.Headers.Add(HttpRequestHeader.Authorization, "Basic " + header);
                Stream os = req.GetRequestStream();
                os.Write(data, 0, data.Length);
                os.Close();
                using (WebResponse resp = req.GetResponse())
                {
                    if (resp != null)
                    {
                        using (var responseStream = resp.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(responseStream))
                            {
                                string strResult = sr.ReadToEnd().Trim();
                                resp.Close();
                                sr.Close();
                                ZohoTokensCls account = JsonConvert.DeserializeObject<ZohoTokensCls>(strResult);
                                var syncSettings = _context.SyncSettings.FirstOrDefault();
                                bool isAdd = false;
                                if (syncSettings == null)
                                {
                                    syncSettings = new SyncSettings();
                                    isAdd = true;
                                }
                                syncSettings.accesstoken = account.access_token;
                                syncSettings.refreshtoken = account.refresh_token;
                                syncSettings.updatedon = DateTime.Now;
                                if (isAdd)
                                    _context.SyncSettings.Add(syncSettings);
                                _context.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["UserData"] = ex;
                return View();
            }
            return RedirectToAction("Index", "home");
        }

        private SyncSettings ZohoRefreshToken()
        {
            try
            {
                var syncSettings = _context.SyncSettings.FirstOrDefault();
                StringBuilder body = new StringBuilder();
                body.Append("grant_type=refresh_token&");
                body.Append("refresh_token=" + syncSettings.refreshtoken + "&");
                body.Append("redirect_uri=" + Uri.EscapeDataString(ZohoRedirectUri) + "&");
                body.Append("client_id=" + ZohoClientId + "&");
                body.Append("client_secret=" + ZohoClientSecret + "&");
                var header = Convert.ToBase64String(new ASCIIEncoding().GetBytes(ZohoClientId + ":" + ZohoClientSecret));
                WebRequest req = WebRequest.Create(ZohoTokenURL);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                byte[] data = Encoding.ASCII.GetBytes(body.ToString());
                req.ContentLength = (long)data.Length;
                req.Headers.Add(HttpRequestHeader.Authorization, "Basic " + header);
                Stream os = req.GetRequestStream();
                os.Write(data, 0, data.Length);
                os.Close();
                using (WebResponse resp = req.GetResponse())
                {
                    if (resp != null)
                    {
                        using (var responseStream = resp.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(responseStream))
                            {
                                string strResult = sr.ReadToEnd().Trim();
                                resp.Close();
                                sr.Close();
                                ZohoTokensCls account = JsonConvert.DeserializeObject<ZohoTokensCls>(strResult);
                                if (account != null && !string.IsNullOrEmpty(account.access_token))
                                {
                                    bool isAdd = false;
                                    if (syncSettings == null)
                                    {
                                        syncSettings = new SyncSettings();
                                        isAdd = true;
                                    }
                                    syncSettings.accesstoken = account.access_token;
                                    //syncSettings.refreshtoken = account.refresh_token;
                                    syncSettings.updatedon = DateTime.Now;
                                    if (isAdd)
                                        _context.SyncSettings.Add(syncSettings);
                                    _context.SaveChanges();
                                    return syncSettings;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return null;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult SyncCustomers()
        {
            var syncSettings = ZohoRefreshToken();
            if (syncSettings != null)
            {
                var accResult = GetZoho(ZohoCustomersURL, syncSettings.accesstoken);
                if (accResult == "401")
                {
                    syncSettings = ZohoRefreshToken();
                    accResult = GetZoho(ZohoCustomersURL, syncSettings.accesstoken);
                }
                if (!accResult.StartsWith("error"))
                {
                    JObject backendObject = JObject.Parse(accResult); // parse json
                    JArray contacts = JArray.Parse(backendObject["contacts"].ToString());
                    //using (MordenDoorsEntities context = new MordenDoorsEntities())
                    //{
                    foreach (var contact in contacts)
                    {
                        string contactid = Convert.ToString(contact["contact_id"]);
                        if (!string.IsNullOrEmpty(contactid))
                        {
                            string contactUrl = $"{ZohoCustomersURL}/{contactid}";
                            var contDetails = GetZoho(contactUrl, syncSettings.accesstoken);
                            if (contDetails == "401")
                            {
                                syncSettings = ZohoRefreshToken();
                                contDetails = GetZoho(contactUrl, syncSettings.accesstoken);
                            }
                            if (!contDetails.StartsWith("error"))
                            {
                                JObject jObject = JObject.Parse(contDetails); // parse json
                                JObject cont = JObject.Parse(jObject["contact"].ToString());
                                var contBillAddress = JObject.Parse(cont["billing_address"].ToString());
                                var contshipAddress = JObject.Parse(cont["shipping_address"].ToString());

                                //var customer = new Customers();
                                var customer = _context.Customers.Where(s => s.ZohoContactId == contactid).FirstOrDefault();
                                var isNew = false;
                                if (customer == null)
                                {
                                    isNew = true;
                                    customer = new Customers();
                                    customer.ZohoContactId = Convert.ToString(cont["contact_id"]);
                                }
                                customer.CompanyName = Convert.ToString(cont["company_name"]);
                                customer.CurrencyRef = Convert.ToString(cont["currency_code"]);
                                customer.DisplayName = Convert.ToString(cont["customer_name"]);
                                customer.Mobile = Convert.ToString(cont["phone"]);
                                customer.CurrencyRef = Convert.ToString(cont["currency_code"]);
                                customer.PrimaryEmailAddr = Convert.ToString(cont["email"]);

                                customer.Active = true;
                                //billing add
                                customer.BillAddrLine1 = Convert.ToString(contBillAddress["address"]);
                                customer.BillAddrLine2 = Convert.ToString(contBillAddress["street2"]);
                                customer.BillAddrCountrySubDivisionCode = Convert.ToString(contBillAddress["state"]);
                                customer.BillAddrPostalCode = Convert.ToString(contBillAddress["zip"]);
                                customer.BillAddrCity = Convert.ToString(contBillAddress["city"]);
                                customer.BillAddrCountry = Convert.ToString(contBillAddress["country"]);
                                //
                                //shipping add
                                customer.ShipAddrLine1 = Convert.ToString(contshipAddress["address"]);
                                customer.ShipAddrLine2 = Convert.ToString(contshipAddress["street2"]);
                                customer.ShipAddrCountrySubDivisionCode = Convert.ToString(contshipAddress["state"]);
                                customer.ShipAddrPostalCode = Convert.ToString(contshipAddress["zip"]);
                                customer.ShipAddrCity = Convert.ToString(contshipAddress["city"]);
                                customer.ShipAddrCountry = Convert.ToString(contshipAddress["country"]);
                                //
                                customer.DefaultTaxCodeRef = Convert.ToString(cont["tax_id"]);

                                customer.LastUpdatedTime = DateTime.Now.ToString();
                                if(isNew)
                                    _context.Customers.Add(customer);
                                _context.SaveChanges();
                            }
                        }
                    }
                    //}
                }
            }
            else
            {
                //Reconnect the Zoho again
            }
            return RedirectToAction("CustomerList", "Customer");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult SyncTaxes()
        {
            var syncSettings = ZohoRefreshToken();
            if (syncSettings != null)
            {
                WebRequest req1 = WebRequest.Create("https://books.zoho.com/api/v3/settings/taxes");
                req1.ContentType = "application/json";
                req1.Method = "GET";
                req1.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + syncSettings.accesstoken);
                using (WebResponse resp1 = req1.GetResponse())
                {
                    if (resp1 != null)
                    {
                        using (var responseStream = resp1.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(responseStream))
                            {
                                string strResult = sr.ReadToEnd().Trim();
                                resp1.Close();
                                sr.Close();
                                //  ViewData["UserData"] = "!!" + accesstoken + "!! ----" + strResult;

                                JObject backendObject = JObject.Parse(strResult); // parse json
                                JToken multipleJson = backendObject["taxes"];

                                if (multipleJson.Count() > 0)
                                {
                                    for (int i = 0; i < multipleJson.Count(); i++)
                                    {
                                        string taxId = Convert.ToString(multipleJson[i]["tax_id"]);
                                        var taxes = _context.Taxes.Where(s => s.ExternalId == taxId).FirstOrDefault();
                                        bool isAdd = false;
                                        if (taxes == null)
                                        {
                                            taxes = new Taxes();
                                            taxes.CreatedOn = DateTime.Now;
                                            taxes.ExternalId = taxId;
                                            isAdd = true;
                                        }
                                        taxes.Name = Convert.ToString(multipleJson[i]["tax_name"]);
                                        taxes.TaxPercentage = Convert.ToDecimal(multipleJson[i]["tax_percentage"]);
                                        taxes.TaxType = Convert.ToString(multipleJson[i]["tax_type"]);
                                        taxes.Country = Convert.ToString(multipleJson[i]["country_code"]);
                                        taxes.UpdatedOn = DateTime.Now;
                                        if (isAdd)
                                            _context.Taxes.Add(taxes);
                                    }
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //Reconnect the Zoho again
            }
            return RedirectToAction("Taxes", "Order");
        }

        public void ZohoCustomers(string accesstoken)
        {
            try
            {
                var contatcResponse = GetZoho(ZohoCustomersURL, accesstoken);
                WebRequest req = WebRequest.Create(ZohoCustomersURL);
                req.ContentType = "application/json";
                req.Method = "GET";
                req.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + accesstoken);
                using (WebResponse resp = req.GetResponse())
                {
                    if (resp != null)
                    {
                        using (var responseStream = resp.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(responseStream))
                            {
                                string strResult = sr.ReadToEnd().Trim();
                                resp.Close();
                                sr.Close();
                                //  ViewData["UserData"] = "!!" + accesstoken + "!! ----" + strResult;

                                // ZohoCustomer objcustomer = JsonConvert.DeserializeObject<ZohoCustomer>(strResult);

                                //    if(objcustomer.contacts.Count>0)
                                //    {
                                //        List<Customers> objcut = new List<Customers>();
                                //        foreach (var cnt in objcustomer.contacts)
                                //        {
                                //              using (MordenDoorsEntities context = new MordenDoorsEntities())
                                //            {

                                //                Customers customers = new Customers();
                                //                customers.CompanyName = cnt.company_name;
                                //                customers.CurrencyRef = cnt.currency_code;
                                //                customers.DisplayName = cnt.customer_name;
                                //                customers.Mobile = cnt.phone;
                                //                customers.CreateTime = DateTime.Now.ToString();

                                //                context.Customers.Add(customers);
                                //                context.SaveChanges();

                                //            }


                                //    }
                                //}
                                //ViewData["UserData"] = objcustomer.contacts[0].contact_name;
                            }
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult SyncProducts()
        {
            var syncSettings = ZohoRefreshToken();
            List<Products> productItems = _userRepository.GetProducts();
            JArray pushData = new JArray();
            if (productItems.Count > 0)
            {
                foreach (var item in productItems)
                {
                    JObject data = new JObject();
                    data["name"] = item.ProductName;
                    data["rate"] = item.Rate;
                    data["description"] = item.ProductDescription;
                    //pushData.Add(data);
                    var result = string.Empty;
                    string operation = "update";
                    if (string.IsNullOrEmpty(item.QbId))
                    {
                        operation = "insert";
                        result = PostZoho(ZohoItemURL, data.ToString(), syncSettings.accesstoken, operation);
                    }
                    else if (!string.IsNullOrEmpty(item.QbId) && item.Resync != null && item.Resync == true)
                        result = PostZoho($"{ZohoItemURL}/{item.QbId}", data.ToString(), syncSettings.accesstoken, operation);
                    if (result == "401")
                    {
                        syncSettings = ZohoRefreshToken();
                        result = PostZoho(ZohoItemURL, data.ToString(), syncSettings.accesstoken, operation);
                    }
                    if (!string.IsNullOrEmpty(result) && !result.StartsWith("error"))
                    {
                        var jResult = JObject.Parse(result);
                        var itemId = JObject.Parse(jResult["item"].ToString())["item_id"].ToString();
                        _userRepository.InsertProductExternalId(item.ProductId, itemId);
                    }
                }

            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult PushInvoice()
        {
            var syncSettings = ZohoRefreshToken();
            List<InvoiceModel> productItems = _userRepository.GetInvoice();
            //var ord = _context.Orders.ToList();
            JArray pushData = new JArray();
            if (productItems.Count > 0)
            {
                JObject data = new JObject();
                JArray line_item = new JArray();
                var currentOId = 0;
                var orderID = 0;
                if (productItems.Count > 0)
                {
                    foreach (var item in productItems)
                    {
                        if (currentOId != item.OrderId)
                        {
                            if (currentOId != 0)
                            {
                                data["line_items"] = line_item;
                                PushInvoice(data, syncSettings, orderID);
                            }

                            data = new JObject();
                            line_item = new JArray();
                            //Order main details here
                            orderID = item.OrderId;
                            data["customer_id"] = item.ZohoCustomerId;
                            data["reference_number"] = item.PO;
                        }

                        currentOId = item.OrderId;
                        //Line item
                        JObject lineitemJson = new JObject();
                        lineitemJson["item_id"] = item.PrdouctZohoId;
                        lineitemJson["description"] = item.Comments;
                        lineitemJson["name"] = item.ProductName;
                        lineitemJson["quantity"] = item.Quantity;
                        lineitemJson["rate"] = (item.Price / item.Quantity);
                        lineitemJson["tax_id"] = (item.Tax);
                        line_item.Add(lineitemJson);

                    }
                    data["line_items"] = line_item;
                    PushInvoice(data, syncSettings, orderID);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        private void PushInvoice(JObject data, SyncSettings syncSettings, int currentOId)
        {
            //pushData.Add(data);
            var result = string.Empty;
            string operation = "insert";
            result = PostZoho(ZohoInvoiceURL, data.ToString(), syncSettings.accesstoken, operation);
            
            if (result == "401")
            {
                syncSettings = ZohoRefreshToken();
                result = PostZoho(ZohoInvoiceURL, data.ToString(), syncSettings.accesstoken, operation);
            }
            if (!string.IsNullOrEmpty(result) && !result.StartsWith("error"))
            {
                var jResult = JObject.Parse(result);
                var invoiceId = JObject.Parse(jResult["invoice"].ToString())["invoice_id"].ToString();
                _userRepository.InsertOderExternalId(currentOId, invoiceId);
            }
        }

        public ActionResult ReSyncProducts()
        {
            var syncSettings = ZohoRefreshToken();
            List<Products> productItems = _userRepository.GetProductsForUpdate();
            JArray pushData = new JArray();
            if (productItems.Count > 0)
            {
                foreach (var item in productItems)
                {
                    JObject data = new JObject();
                    data["name"] = item.ProductName;
                    data["rate"] = item.Rate;
                    data["description"] = item.ProductDescription;
                    string url = $"{ZohoItemURL}/{item.QbId}";
                    var result = PostZoho(url, data.ToString(), syncSettings.accesstoken, "update");
                    if (result == "401")
                    {
                        syncSettings = ZohoRefreshToken();
                        result = PostZoho(url, data.ToString(), syncSettings.accesstoken, "update");
                    }
                    if (string.IsNullOrEmpty(result) && !result.StartsWith("error"))
                    {
                        var jResult = JObject.Parse(result);
                        var itemId = JObject.Parse(jResult["item"].ToString())["item_id"].ToString();
                        _userRepository.ProductResyncDone(item.ProductId, itemId);
                    }
                }

            }
            return RedirectToAction("Index", "Home");
        }
        public string PostZoho(string url, string postData, string accessToken, string operation)
        {
            string result = string.Empty;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);

                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = operation == "insert" ? "POST" : "PUT";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                if (!string.IsNullOrEmpty(accessToken))
                    request.Headers.Add($"Authorization: Bearer {accessToken}");
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseString = reader.ReadToEnd();
                    response.Close();
                    return responseString;
                }

            }
            catch (WebException wex)
            {
                return "error";
            }
        }

        public string GetZoho(string url, string accessToken)
        {
            var result = string.Empty;
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                if (!string.IsNullOrEmpty(accessToken))
                    req.Headers.Add($"Authorization: Bearer {accessToken}");
                req.Headers.Add("Cache-Control: no-cache");
                req.ContentType = "application/json";
                req.Method = "GET";
                req.Timeout = 30000;//30 seconds
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                    response.Close();
                    return result;
                }
            }
            catch (WebException wex)
            {
                return "error";
            }
        }
    }

    public class ZohoTokensCls
    {
        public string id_token
        {
            get;
            set;
        }
        public string access_token
        {
            get;
            set;
        }
        public string refresh_token
        {
            get;
            set;
        }
        public int expires_in
        {
            get;
            set;
        }
        public string token_type
        {
            get;
            set;
        }
    }
}
