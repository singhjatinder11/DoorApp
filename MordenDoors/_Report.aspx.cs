using Microsoft.Reporting.WebForms;
using MordenDoors.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace MordenDoors
{
    public partial class _Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int orderId = 0;
            if (!string.IsNullOrEmpty(Request.QueryString["orderid"]))
            {
                orderId = Convert.ToInt32(Request.QueryString["orderid"]);
            }
            lbloader.Visible = true;

            if (IsPostBack == false)
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/Report/Report1.rdlc");
                DataTable dt = getdetail(orderId);
                if (dt.Rows.Count > 0)
                {
                    string address = Convert.ToString(dt.Rows[0]["AddressLine1"]) + "," + Convert.ToString(dt.Rows[0]["City"]) + "," + Convert.ToString(dt.Rows[0]["PinCode"]) + "," + Convert.ToString(dt.Rows[0]["Country"]);
                    var param1 = new ReportParameter("orderno", Convert.ToString(dt.Rows[0]["Id"]));
                    var param2 = new ReportParameter("customername", Convert.ToString(dt.Rows[0]["CompanyName"]));
                    var param3 = new ReportParameter("status", Convert.ToString(dt.Rows[0]["orderStatus"]));
                    var param4 = new ReportParameter("orderdate", Convert.ToDateTime(dt.Rows[0]["CretaedOn"]).ToString("dd/MM/yyyy"));
                    var param5 = new ReportParameter("duedate", Convert.ToDateTime(dt.Rows[0]["DeliveryTime"]).ToString("dd/MM/yyyy"));
                    var param6 = new ReportParameter("Total", Convert.ToString(dt.Rows[0]["TotalAmount"]));
                    var param7 = new ReportParameter("PO", Convert.ToString(dt.Rows[0]["PO"]));
                    var param8 = new ReportParameter("dilveryaddress", address);
                    var param9 = new ReportParameter("invoiceno", Convert.ToString(dt.Rows[0]["TrackingID"]));
                    var param10 = new ReportParameter("cutomerprice", Convert.ToString(dt.Rows[0]["customerprice"]));
                    var param11 = new ReportParameter("payableamount", Convert.ToString(dt.Rows[0]["PayableAmount"]));
                    ReportViewer1.LocalReport.SetParameters(new[] { param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11 });
                }
                DataTable orderItemTable = new DataTable();
                orderItemTable.Columns.Add("ProductName", typeof(string));
                orderItemTable.Columns.Add("Quantity", typeof(string));
                orderItemTable.Columns.Add("Height", typeof(string));
                orderItemTable.Columns.Add("Width", typeof(string));
                orderItemTable.Columns.Add("TotalPrice", typeof(string));

                using (MordenDoorsEntities context = new MordenDoorsEntities())
                {
                    var orderItems = context.reportorder(orderId).ToList();
                    foreach (var item in orderItems.Where(x => x.OrderSubItemId == null))
                    {
                        DataRow itemDr = orderItemTable.NewRow();
                        itemDr["ProductName"] = item.ProductName;
                        itemDr["Quantity"] = item.Quantity;
                        itemDr["Height"] = item.Height;
                        itemDr["Width"] = item.Width;
                        itemDr["TotalPrice"] = item.TotalPrice;
                        orderItemTable.Rows.Add(itemDr);
                        foreach (var subItem in orderItems.Where(x=> x.OrderSubItemId != null && x.OrderItemId==item.OrderItemId))
                        {
                            DataRow subItemDr = orderItemTable.NewRow();
                            subItemDr["ProductName"] = " - "+subItem.ProductName;
                            subItemDr["Quantity"] = subItem.Quantity == null ? (object)DBNull.Value : subItem.Quantity;
                            subItemDr["Height"] = subItem.Height == null ? (object)DBNull.Value : subItem.Quantity;
                            subItemDr["Width"] = subItem.Width == null ? (object)DBNull.Value : subItem.Quantity;
                            subItemDr["TotalPrice"] = subItem.TotalPrice == null ? (object)DBNull.Value : subItem.Quantity;
                            orderItemTable.Rows.Add(subItemDr);
                        }
                    }
                }

                ReportDataSource rds = new ReportDataSource("DTS", orderItemTable);
                ReportViewer1.LocalReport.DataSources.Clear();
                //Add ReportDataSource  
                ReportViewer1.LocalReport.DataSources.Add(rds);

                lbloader.Visible = false;
            }


        }
        //DataTable newtable(string orderid)
        //{
        //    DataTable data = new DataTable();
        //    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
        //    {
        //        SqlCommand cmd = new SqlCommand("reportorder", con);
        //        cmd.Parameters.Add("@orderid", orderid);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //        adp.Fill(data);


        //    }



        //        return data;
        //}

        DataTable getdetail(int id)
        {
            DataTable data = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string cmdtxt = @"select Customers.CompanyName,Orders.Id,Orders.CretaedOn,Orders.DeliveryTime,orderStatus.orderStatus,orders.PO,orders.AddressLine1,orders.TrackingID,orders.City,orders.Country,orders.PinCode,orders.customerprice,orders.PayableAmount,orders.TotalAmount from orders join Customers on orders.CustomerId=Customers.Id  join OrderStatus on Orders.StatusId=orderStatus.Id where Orders.Id='" + id + "'";
                SqlDataAdapter adp = new SqlDataAdapter(cmdtxt, con);
                adp.Fill(data);
            }
            return data;


        }
        DataTable gettotal(int id)
        {
            DataTable data = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string cmdtxt = @"select SUM(Quantity)as total from OrderItems where OrderId='" + id + "'";
                SqlDataAdapter adp = new SqlDataAdapter(cmdtxt, con);
                adp.Fill(data);
            }
            return data;


        }
    }
}