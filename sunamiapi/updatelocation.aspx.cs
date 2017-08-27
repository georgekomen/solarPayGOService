using sunamiapi.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace sunamiapi.Controllers
{
    public partial class updatelocation : System.Web.UI.Page
    {
        private string imei;
        private string lat;
        private string lon;
        private int co;
        db_a0a592_sunamiEntities se = new db_a0a592_sunamiEntities();
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Response.Write("starting......");
                imei = Request.QueryString["imei"].Replace(@"'", @"");
                lat = Request.QueryString["lat"].Replace(@"'", @"");
                lon = Request.QueryString["lon"].Replace(@"'", @"");
                //the 2000 is for varying interval at client side

                //check that lat and lon are not blank
                if (lat != "" && lat != null)
                {
                    try
                    {
                        //check if this imei exists int the db
                        co = se.tbl_system.Where(o => o.imei_number == imei).Count();
                    }
                    catch
                    {
                        co = 0;
                    }
                    if (co >= 1)
                    {
                        //save current location
                        //find entry id of customer with this imei active
                        tbl_system ts = se.tbl_system.FirstOrDefault(o => o.imei_number == imei);
                        ts.current_lat = lat;
                        ts.current_lon = lon;
                        ts.last_connected_to_db_date = DateTime.Now;
                        se.SaveChanges();
                        bool? response = ts.active_status;
                        if (response == true)
                        {
                            Response.Clear();
                            Response.Write("*1#*0#*+254713014492#");//on/off , post interval(0-10mins) , gsm reset , sms feedback number
                            Response.End();
                        }
                        if (response == false)
                        {
                            Response.Clear();
                            Response.Write("*0#*0#*+254713014492#");//on/off , post interval(0-10mins) , gsm reset , sms feedback number
                            Response.End();
                        }
                    }
                    else
                    {

                        try
                        {
                            int? f = se.tbl_imei_sim_numbers.Where(h => h.imei == imei).Count();
                            if (f < 1)
                            {
                                //record imei in tbl_imei_sim_numbers
                                tbl_imei_sim_numbers imeiR = new tbl_imei_sim_numbers();
                                imeiR.imei = imei;
                                se.tbl_imei_sim_numbers.Add(imeiR);
                                se.SaveChanges();
                                Response.Clear();
                                Response.Write("*0#*0#*+254713014492#smsd$Success%s*connected to server#e");//on/off , post interval(0-10mins) , gsm reset , sms feedback number
                                Response.End();
                            }
                            else
                            {
                                Response.Clear();
                                Response.Write("*0#*0#*+254713014492#smsd$Success%s*connected to server#e");//on/off , post interval(0-10mins) , gsm reset , sms feedback number
                                Response.End();
                            }


                        }
                        catch (Exception j)
                        {

                        }
                    }
                }
                else
                {
                    //Response.Write("lat & lon empty");
                }
            }
            catch (Exception i)
            {
                //Response.Write(i.Message);
            }
        }
    }
}