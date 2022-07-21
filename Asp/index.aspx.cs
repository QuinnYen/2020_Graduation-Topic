using System;
using System.Data;
using System.Data.SQLite;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
// using System.Windows.Forms;

namespace E_Final_KeyShow
{
    public partial class index : System.Web.UI.Page
    {
        public List<string> test_list;
        public string test_id;

        // 頁面讀取
        protected void Page_Load(object sender, EventArgs e)
        {
            var conn = new SQLiteConnection("Data Source='E:/Project/E_Final_KeyShow/RES/python_E_Final.db';Version=3;", true);
            var db_str_1 = "select count (keywords) from keys";
            var db_str_2 = "select subpart from keys WHERE second_ratio < 0.5 GROUP by subpart ORDER by second_ratio DESC LIMIT 50;";

            // 列出資料庫筆數
            try
            {
                // 執行資料庫
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(db_str_1, conn);
                string record = cmd.ExecuteScalar().ToString();
                cmd.Dispose(); conn.Close();

                // 呈現結果
                In_00.Text = record;
            }
            catch { In_00.Text = "不知道有幾種"; }

            // 生成subpart按鈕
            var show_subpart_list = new List<string>();
            try
            {
                // 執行資料庫
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(db_str_2, conn);
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    show_subpart_list.Add(rdr["subpart"].ToString());
                }
                rdr.Close(); cmd.Dispose(); conn.Close();

                // 呈現結果
                rpt_sub_list.DataSource = show_subpart_list;
                rpt_sub_list.DataBind();

            }
            catch (Exception)
            { }

        }

        // 自動建議
        [ScriptMethod()]
        [WebMethod]
        public static string[] GetKeywords(string prefixText)
        {
            ArrayList keyResult = new ArrayList();
            var db_str = string.Format("select * from keys where keywords Like '%{0}%' GROUP by keywords ORDER by keywords DESC, second_ratio DESC", prefixText);
            var con = new SQLiteConnection("Data Source='E:/Project/E_Final_KeyShow/RES/python_E_Final.db';Version=3;", true);

            try
            {
                // 執行資料庫
                con.Open();
                SQLiteCommand cmd = new SQLiteCommand(db_str, con);
                cmd.Parameters.AddWithValue("@prefixText", prefixText);
                SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    keyResult.Add(dt.Rows[i]["keywords"].ToString());
                }
                cmd.Dispose(); con.Close();
                return (string[])keyResult.ToArray(typeof(string));
            }
            catch { return (string[])keyResult.ToArray(typeof(string)); }
            
        }

        // 搜尋程式
        protected void SearchBar(object sender, EventArgs e)
        {
            //引用stopwatch物件
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset(); //碼表歸零
            sw.Start(); //碼表開始計時

            //區塊顯示
            Panel1.Visible = true;

            // 資料庫連結
            string a = string.Format(@"Data Source={0};Version=3;", "E:/Project/E_Final_KeyShow/RES/python_E_Final.db");
            var conn = new SQLiteConnection(a, true);

            // 區域宣告
            string db_str = "";
            var tmp01 = "";
            var tmp06 = "";
            
            var depart_list = new List<string>();
            var subpart_llist = new List<string>();
            var web_list = new List<string>();
            var sentence_list = new List<string>();

            // 取得使用者輸入內容
            var search = TextBox1.Text;
            In_02.Text = search;
            string[] tmp = search.Split(' ');

            // 整理資料搜尋語句
            for (int i = 0; i < tmp.Length; i++)
            {
                var tmp05 = string.Format("(keywords Like '%{0}%') AND ", tmp[i]);
                var tmp04 = string.Format("(keywords Like '%{0}%' Or depart Like '%{0}%' Or subpart Like '%{0}%') Or ", tmp[i], tmp[i], tmp[i]);
                tmp06 += tmp05;
                tmp01 += tmp04;
            }
            var search_1 = tmp06.Substring(0, tmp06.Length - 5);
            var search_2 = tmp01.Substring(0, tmp01.Length - 4);

            // 模糊搜尋
            if (!String.IsNullOrEmpty(TextBox1.Text))
            {
                db_str = "" +
                "SELECT * FROM keys Where " + search_1 + " GROUP by website UNION " +
                "SELECT * FROM keys Where " + search_2 + " GROUP by website ORDER by keywords DESC, second_ratio DESC LIMIT 200";
            }

            // 嘗試執行資料庫
            try
            {
                // 執行資料庫
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(db_str, conn);
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    depart_list.Add(rdr["depart"].ToString());
                    subpart_llist.Add(rdr["subpart"].ToString());
                    web_list.Add(rdr["website"].ToString());
                    sentence_list.Add(rdr["sentence"].ToString());
                }
                rdr.Close(); cmd.Dispose(); conn.Close();

            }
            catch (Exception ex)
            {
                In_01.Text = "0";
                In_02.Text = "ERROR";
                In_03.Text += db_str + "<br/>" + ex.ToString();
            }

            // 相關-整理
            var show_keywords_list = new List<string>();
            var show_search_list = new List<string>();

            for (int i = 0; i < subpart_llist.Count; i++)
            {
                // 相關關鍵詞-整理
                var tmp01_list = new List<string>();
                var db_keywords = string.Format("SELECT keywords FROM keys WHERE subpart = '{0}' GROUP by keywords ORDER by second_ratio DESC LIMIT 10;", subpart_llist[i]);
                try
                {
                    // 執行資料庫
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(db_keywords, conn);
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        tmp01_list.Add(rdr["keywords"].ToString());
                    }
                    rdr.Close(); cmd.Dispose(); conn.Close();
                }
                catch (Exception ) { }

                string tmp02_str = String.Join(", ", tmp01_list.ToArray());
                show_keywords_list.Add(tmp02_str);


                // 相關搜尋-整理
                var tmp03_list = new List<string>();
                var tmp04_list = new List<string>();
                var tmp05_list = new List<string>();
                var tmp06_str = "";

                var tmp07_str = subpart_llist[i].Substring(0, 2);

                var db_search = string.Format("" +
                    "SELECT depart, subpart, website " +
                    "FROM(SELECT * FROM keys Where(keywords Like '%{0}%' Or subpart like '%{0}%') AND (depart IN ('{1}')) ORDER by second_ratio DESC) " +
                    "WHERE second_ratio < 0.5 " +
                    "GROUP by website ORDER by second_ratio DESC LIMIT 3;", tmp07_str, depart_list[i]);
                try
                {
                    // 執行資料庫
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(db_search, conn);
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        tmp03_list.Add(rdr["depart"].ToString());
                        tmp04_list.Add(rdr["subpart"].ToString());
                        tmp05_list.Add(rdr["website"].ToString());
                    }
                    rdr.Close(); cmd.Dispose(); conn.Close();
                }
                catch (Exception) { }

                for (int j = 0; j < tmp03_list.Count; j++)
                {
                    tmp06_str += string.Format("" +
                        "<a href='{0}'class='link wipe' target='_blank'> {1} - {2} </a>", tmp05_list[j], tmp03_list[j], tmp04_list[j]);
                }

                show_search_list.Add(tmp06_str);

            }

            // 呈現結果
            rpt_SelectNext.DataSource = depart_list.Distinct().ToList();
            rpt_SelectNext.DataBind();

            In_01.Text = depart_list.Count.ToString();
            for (int i = 0; i < depart_list.Count; i++)
            {
                In_03.Text += string.Format("" +
                    "<div class='search-result'>" +
                    "<h3><a href='{0}' target='_blank'>{1} - {2}</a></h3>" +
                    "<a href='{3}' target='_blank' class='search-link'>{4}</a>" +
                    "<div class='wrapper'><div class='accordion_wrap accordion_1'>" +
                    "<div class='accordion_header'>{5}</div>" +
                    "<div class='accordion_body'>" +
                    "<p>網站關鍵詞：{6}<br/><br/>" +
                    "熱門相關連結：<br/>{7}</p>" +
                    "</div></div></div>" +
                    "<div class='hr-line-dashed'></div>"
                    , web_list[i], depart_list[i], subpart_llist[i], web_list[i], web_list[i], sentence_list[i], show_keywords_list[i], show_search_list[i]);
            }

            //顯示時間
            sw.Stop(); //碼錶停止
            In_04.Text = sw.Elapsed.TotalSeconds.ToString();
        }

        // 單位按鈕
        protected void unityBtn(object sender, EventArgs e)
        {
            Panel1.Visible = false;
            //重製ViewState
            Page.EnableViewState = false;

            // 宣告變數
            int unity_bool = 0;
            var db_str = "";

            Button btn = (Button)sender;
            if (btn == acad_btn)
            {
                // 資料語句
                db_str = "SELECT depart, website FROM keys WHERE subpart = '學術' GROUP by depart ORDER by id ASC;";

                // Trigger動作
                unity_bool = 1;
                Unity_db(unity_bool, db_str);
                Panel2.Visible = true; Panel3.Visible = false; Panel4.Visible = false;
                acad_btn.BackColor = Color.Red; offic_btn.BackColor = default(Color); other_btn.BackColor = default(Color);
            }
            if (btn == offic_btn)
            {
                // 資料語句
                db_str = "SELECT depart, website FROM keys WHERE subpart = '行政' GROUP by depart ORDER by id ASC;";

                // Trigger動作
                unity_bool = 2;
                Unity_db(unity_bool, db_str);
                Panel2.Visible = false; Panel3.Visible = true; Panel4.Visible = false;
                acad_btn.BackColor = default(Color); offic_btn.BackColor = Color.Red; other_btn.BackColor = default(Color);
            }
            if (btn == other_btn)
            {
                // 資料語句
                db_str = "SELECT depart, website FROM keys WHERE subpart = '其他單位' GROUP by depart ORDER by id ASC;";

                // Trigger動作
                unity_bool = 3;
                Unity_db(unity_bool, db_str);
                Panel2.Visible = false; Panel3.Visible = false; Panel4.Visible = true;
                acad_btn.BackColor = default(Color); offic_btn.BackColor = default(Color); other_btn.BackColor = Color.Red;
            }

        }
        
        // 單位資料庫連動
        public void Unity_db(int unity_bool, string db_str)
        {
            // 資料庫連結
            string a = string.Format(@"Data Source={0};Version=3;", "E:/Project/E_Final_KeyShow/RES/python_E_Final.db");
            var conn = new SQLiteConnection(a, true);

            int[] cont = { 8, 8, 6, 7, 5, 2, 2 };
            var depart = new List<string>();
            var web = new List<string>();

            // 嘗試執行資料庫
            try
            {
                // 執行資料庫
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(db_str, conn);
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    depart.Add(rdr["depart"].ToString());
                    web.Add(rdr["website"].ToString());
                }
                rdr.Close(); cmd.Dispose(); conn.Close();

                // 宣告變數
                int rout = 0, hover = 0;

                // 學術單位
                if (unity_bool == 1)
                {
                    foreach (int tmp in cont)
                    {
                        if (rout == 0)
                        {
                            for (int i = 0; i < tmp; i++)
                            {
                                In_unity_0.Text += string.Format("" +
                                    "<div class='link'>" +
                                    "<a href='{0}'class='link wipe' target='_blank'> {1} </a>" +
                                    "</div>", web[hover], depart[hover]);
                                hover += 1;
                            }
                            rout += 1;
                            continue;
                        }

                        if (rout == 1)
                        {
                            for (int qq = 0; qq < tmp; qq++)
                            {
                                In_unity_1.Text += string.Format("" +
                                    "<div class='link'>" +
                                    "<a href='{0}'class='link wipe' target='_blank'> {1} </a>" +
                                    "</div>", web[hover], depart[hover]);
                                hover += 1;
                            }
                            rout += 1;
                            continue;
                        }

                        if (rout == 2)
                        {
                            for (int qq = 0; qq < tmp; qq++)
                            {
                                In_unity_2.Text += string.Format("" +
                                    "<div class='link'>" +
                                    "<a href='{0}'class='link wipe' target='_blank'> {1} </a>" +
                                    "</div>", web[hover], depart[hover]);
                                hover += 1;
                            }
                            rout += 1;
                            continue;
                        }

                        if (rout == 3)
                        {
                            for (int qq = 0; qq < tmp; qq++)
                            {
                                In_unity_3.Text += string.Format("" +
                                    "<div class='link'>" +
                                    "<a href='{0}'class='link wipe' target='_blank'> {1} </a>" +
                                    "</div>", web[hover], depart[hover]);
                                hover += 1;
                            }
                            rout += 1;
                            continue;
                        }

                        if (rout == 4)
                        {
                            for (int qq = 0; qq < tmp; qq++)
                            {
                                In_unity_4.Text += string.Format("" +
                                    "<div class='link'>" +
                                    "<a href='{0}'class='link wipe' target='_blank'> {1} </a>" +
                                    "</div>", web[hover], depart[hover]);
                                hover += 1;
                            }
                            rout += 1;
                            continue;
                        }

                        if (rout == 5)
                        {
                            for (int qq = 0; qq < tmp; qq++)
                            {
                                In_unity_5.Text += string.Format("" +
                                    "<div class='link'>" +
                                    "<a href='{0}'class='link wipe' target='_blank'> {1} </a>" +
                                    "</div>", web[hover], depart[hover]);
                                hover += 1;
                            }
                            rout += 1;
                            continue;
                        }

                        if (rout == 6)
                        {
                            for (int qq = 0; qq < tmp; qq++)
                            {
                                In_unity_6.Text += string.Format("" +
                                    "<div class='link'>" +
                                    "<a href='{0}'class='link wipe' target='_blank'> {1} </a>" +
                                    "</div>", web[hover], depart[hover]);
                                hover += 1;
                            }
                            rout += 1;
                            continue;
                        }
                        break;
                    }
                }

                // 行政單位
                if (unity_bool == 2)
                {
                    for (int i = 0; i < depart.Count; i++)
                    {
                        In_unity_7.Text += string.Format("" +
                            "<div class='link'>" +
                            "<a href='{0}'class='link wipe' target='_blank'> {1} </a>" +
                            "</div>", web[i], depart[i]);
                    }
                }

                // 其他單位
                if (unity_bool == 3)
                {
                    for (int i = 0; i < depart.Count; i++)
                    {
                        In_unity_8.Text += string.Format("" +
                            "<div class='link'>" +
                            "<a href='{0}'class='link wipe' target='_blank'> {1} </a>" +
                            "</div>", web[i], depart[i]);
                    }
                }

            }
            catch (Exception)
            {
                Panel2.Visible = false;
                test.Text = "無顯示結果";
            }

        }

        // Test
        protected void SearchMain(int Target)
        {
            if (Target == 1)
            {

            }

            if (Target == 2)
            {

            }
        }

        // 子分支(側邊攔)按鈕
        protected void sub_list_click(object sender, EventArgs e)
        {
            //SearchMain(1);
            //引用stopwatch物件
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset(); //碼表歸零
            sw.Start(); //碼表開始計時

            //區塊顯示
            Panel1.Visible = true;

            // 資料庫連結
            string a = string.Format(@"Data Source={0};Version=3;", "E:/Project/E_Final_KeyShow/RES/python_E_Final.db");
            var conn = new SQLiteConnection(a, true);

            // 區域宣告
            var search = "";
            var tmp01 = "";
            var tmp06 = "";

            var depart_list = new List<string>();
            var subpart_llist = new List<string>();
            var web_list = new List<string>();
            var sentence_list = new List<string>();

            // 取得使用者輸入內容
            Button btn = (Button)sender;
            search = btn.Text;
            In_02.Text = search;
            TextBox1.Text = search;
            string[] tmp = search.Split(' ');

            // 整理資料搜尋語句
            for (int i = 0; i < tmp.Length; i++)
            {
                // 子分支搜尋字串
                var tmp05 = string.Format("(keywords Like '%{0}%') AND ", tmp[i]);
                var tmp04 = string.Format("(keywords Like '%{0}%' Or depart Like '%{0}%' Or subpart Like '%{0}%') Or ", tmp[i], tmp[i], tmp[i]);
                tmp06 += tmp05;
                tmp01 += tmp04;
            }
            var search_1 = tmp06.Substring(0, tmp06.Length - 5);
            var search_2 = tmp01.Substring(0, tmp01.Length - 4);

            // 子分支搜尋
            string db_str = "" +
                "SELECT * FROM keys Where " + search_1 + " GROUP by website UNION " +
                "SELECT * FROM keys Where " + search_2 + " GROUP by website ORDER by keywords DESC, second_ratio DESC LIMIT 200";

            // 嘗試執行資料庫
            try
            {
                // 執行資料庫
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(db_str, conn);
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    depart_list.Add(rdr["depart"].ToString());
                    subpart_llist.Add(rdr["subpart"].ToString());
                    web_list.Add(rdr["website"].ToString());
                    sentence_list.Add(rdr["sentence"].ToString());
                }
                rdr.Close(); cmd.Dispose(); conn.Close();

            }
            catch (Exception ex)
            {
                In_01.Text = "0";
                In_02.Text = "ERROR";
                In_03.Text += db_str + "<br/>" + ex.ToString();
            }

            // 相關-整理
            var show_keywords_list = new List<string>();
            var show_search_list = new List<string>();

            for (int i = 0; i < subpart_llist.Count; i++)
            {
                // 相關關鍵詞-整理
                var tmp01_list = new List<string>();
                var db_keywords = string.Format("SELECT keywords FROM keys WHERE subpart = '{0}' GROUP by keywords ORDER by second_ratio DESC LIMIT 10;", subpart_llist[i]);
                try
                {
                    // 執行資料庫
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(db_keywords, conn);
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        tmp01_list.Add(rdr["keywords"].ToString());
                    }
                    rdr.Close(); cmd.Dispose(); conn.Close();
                }
                catch (Exception) { }

                string tmp02_str = String.Join(", ", tmp01_list.ToArray());
                show_keywords_list.Add(tmp02_str);


                // 相關搜尋-整理
                var tmp03_list = new List<string>();
                var tmp04_list = new List<string>();
                var tmp05_list = new List<string>();
                var tmp06_str = "";

                var tmp07_str = subpart_llist[i].Substring(0, 2);

                var db_search = string.Format("" +
                    "SELECT depart, subpart, website " +
                    "FROM(SELECT * FROM keys Where(keywords Like '%{0}%' Or subpart like '%{0}%') AND (depart IN ('{1}')) ORDER by second_ratio DESC) " +
                    "WHERE second_ratio < 0.5 " +
                    "GROUP by website ORDER by second_ratio DESC LIMIT 3;", tmp07_str, depart_list[i]);
                try
                {
                    // 執行資料庫
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(db_search, conn);
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        tmp03_list.Add(rdr["depart"].ToString());
                        tmp04_list.Add(rdr["subpart"].ToString());
                        tmp05_list.Add(rdr["website"].ToString());
                    }
                    rdr.Close(); cmd.Dispose(); conn.Close();
                }
                catch (Exception) { }

                for (int j = 0; j < tmp03_list.Count; j++)
                {
                    tmp06_str += string.Format("" +
                        "<a href='{0}'class='link wipe' target='_blank'> {1} - {2} </a>", tmp05_list[j], tmp03_list[j], tmp04_list[j]);
                }

                show_search_list.Add(tmp06_str);

            }

            // 呈現結果
            rpt_SelectNext.DataSource = depart_list.Distinct().ToList();
            rpt_SelectNext.DataBind();

            In_01.Text = depart_list.Count.ToString();
            for (int i = 0; i < depart_list.Count; i++)
            {
                In_03.Text += string.Format("" +
                    "<div class='search-result'>" +
                    "<h3><a href='{0}' target='_blank'>{1} - {2}</a></h3>" +
                    "<a href='{3}' target='_blank' class='search-link'>{4}</a>" +
                    "<div class='wrapper'><div class='accordion_wrap accordion_1'>" +
                    "<div class='accordion_header'>{5}</div>" +
                    "<div class='accordion_body'>" +
                    "<p>網站關鍵詞：{6}<br/><br/>" +
                    "熱門相關連結：<br/>{7}</p>" +
                    "</div></div></div>" +
                    "<div class='hr-line-dashed'></div>"
                    , web_list[i], depart_list[i], subpart_llist[i], web_list[i], web_list[i], sentence_list[i], show_keywords_list[i], show_search_list[i]);
            }

            //顯示時間
            sw.Stop(); //碼錶停止
            In_04.Text = sw.Elapsed.TotalSeconds.ToString();
        }

        // Repeater使用
        protected void rpt_SelectNext_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var text = e.Item.DataItem.ToString().Trim();
                CheckBox chk_SelectNext = (CheckBox)e.Item.FindControl("chk_SelectNext");
                chk_SelectNext.Text = text;
            }
        }
        protected void rpt_sub_list_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //string copa_payer = ((DataRowView)e.Item.DataItem)["copa_payer"].ToString().Trim();
                var text = e.Item.DataItem.ToString().Trim();
                Button btn_sub_list = (Button)e.Item.FindControl("btn_sub_list");
                btn_sub_list.Text = text;
            }
        }

        // 分支搜尋-確認變更
        protected void chk_SelectNext_CheckedChanged(object sender, EventArgs e)
        {
            var targetNext = new List<string>();
            for (var i = 0; i < rpt_SelectNext.Items.Count; i++)
            {
                CheckBox chk_SelectNext = (CheckBox)rpt_SelectNext.Items[i].FindControl("chk_SelectNext");
                if (chk_SelectNext.Checked)
                    targetNext.Add(chk_SelectNext.Text.Trim());
            }
            SearchNext(targetNext);
        }

        // 分支搜尋-主搜尋
        protected void SearchNext(List<string> targetNext)
        {
            //引用stopwatch物件
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset(); //碼表歸零
            sw.Start(); //碼表開始計時

            //區塊顯示
            Panel1.Visible = true;

            // 資料庫連結
            string a = string.Format(@"Data Source={0};Version=3;", "E:/Project/E_Final_KeyShow/RES/python_E_Final.db");
            var conn = new SQLiteConnection(a, true);

            // 區域宣告
            string db_str = "";
            var tmp01 = "";
            var tmp02 = "";

            var depart_list = new List<string>();
            var subpart_llist = new List<string>();
            var web_list = new List<string>();
            var sentence_list = new List<string>();

            // 取得使用者輸入內容
            var search = TextBox1.Text;
            In_02.Text = search;
            string[] tmp = search.Split(' ');

            // 整理資料搜尋語句
            if (targetNext.Count != 0)
            {
                for (int i = 0; i < tmp.Length; i++)
                {
                    var key_and = string.Format("(keywords Like '%{0}%' Or subpart Like '%{0}%') AND ", tmp[i]);
                    tmp01 += key_and;
                }
                var search_1 = tmp01.Substring(0, tmp01.Length - 5);

                for (int i = 0; i < targetNext.Count; i++)
                {
                    var dep_in = string.Format("'{0}',", targetNext[i]);
                    tmp02 += dep_in;
                }
                var search_2 = tmp02.Substring(0, tmp02.Length - 1);

                if (!String.IsNullOrEmpty(TextBox1.Text))
                {
                    db_str = "" +
                    "SELECT * FROM keys Where " + search_1 + " AND (depart IN(" + search_2 + ")) GROUP by website ORDER by keywords DESC, second_ratio DESC LIMIT 200;";
                }
            }
            else if (targetNext.Count == 0)
            {
                for (int i = 0; i < tmp.Length; i++)
                {
                    var key_and = string.Format("(keywords Like '%{0}%') AND ", tmp[i]);
                    var uni_or = string.Format("(keywords Like '%{0}%' Or depart Like '%{0}%' Or subpart Like '%{0}%') Or ", tmp[i], tmp[i], tmp[i]);
                    tmp01 += key_and;
                    tmp02 += uni_or;
                }
                var search_1 = tmp01.Substring(0, tmp01.Length - 5);
                var search_2 = tmp02.Substring(0, tmp02.Length - 4);

                if (!String.IsNullOrEmpty(TextBox1.Text))
                {
                    db_str = "" +
                    "SELECT * FROM keys Where " + search_1 + " GROUP by website UNION " +
                    "SELECT * FROM keys Where " + search_2 + " GROUP by website ORDER by keywords DESC, second_ratio DESC LIMIT 200";
                }
            }

            // 嘗試執行資料庫
            try
            {
                // 執行資料庫
                conn.Open();
                SQLiteCommand cmd = new SQLiteCommand(db_str, conn);
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    depart_list.Add(rdr["depart"].ToString());
                    subpart_llist.Add(rdr["subpart"].ToString());
                    web_list.Add(rdr["website"].ToString());
                    sentence_list.Add(rdr["sentence"].ToString());
                }
                rdr.Close(); cmd.Dispose(); conn.Close();

                
            }
            catch (Exception ex)
            {
                In_01.Text = "0";
                In_02.Text = "ERROR";
                In_03.Text += db_str + "<br/>" + ex.ToString();
            }

            // 相關-整理
            var show_keywords_list = new List<string>();
            var show_search_list = new List<string>();

            for (int i = 0; i < subpart_llist.Count; i++)
            {
                // 相關關鍵詞-整理
                var tmp01_list = new List<string>();
                var db_keywords = string.Format("SELECT keywords FROM keys WHERE subpart = '{0}' GROUP by keywords ORDER by second_ratio DESC LIMIT 10;", subpart_llist[i]);
                try
                {
                    // 執行資料庫
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(db_keywords, conn);
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        tmp01_list.Add(rdr["keywords"].ToString());
                    }
                    rdr.Close(); cmd.Dispose(); conn.Close();
                }
                catch (Exception) { }

                string tmp02_str = String.Join(", ", tmp01_list.ToArray());
                show_keywords_list.Add(tmp02_str);


                // 相關搜尋-整理
                var tmp03_list = new List<string>();
                var tmp04_list = new List<string>();
                var tmp05_list = new List<string>();
                var tmp06_str = "";

                var tmp07_str = subpart_llist[i].Substring(0, 2);

                var db_search = string.Format("" +
                    "SELECT depart, subpart, website " +
                    "FROM(SELECT * FROM keys Where(keywords Like '%{0}%' Or subpart like '%{0}%') AND (depart IN ('{1}')) ORDER by second_ratio DESC) " +
                    "WHERE second_ratio < 0.5 " +
                    "GROUP by website ORDER by second_ratio DESC LIMIT 3;", tmp07_str, depart_list[i]);
                try
                {
                    // 執行資料庫
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(db_search, conn);
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        tmp03_list.Add(rdr["depart"].ToString());
                        tmp04_list.Add(rdr["subpart"].ToString());
                        tmp05_list.Add(rdr["website"].ToString());
                    }
                    rdr.Close(); cmd.Dispose(); conn.Close();
                }
                catch (Exception) { }

                for (int j = 0; j < tmp03_list.Count; j++)
                {
                    tmp06_str += string.Format("" +
                        "<a href='{0}'class='link wipe' target='_blank'> {1} - {2} </a>", tmp05_list[j], tmp03_list[j], tmp04_list[j]);
                }

                show_search_list.Add(tmp06_str);
            }
            
            // 呈現結果
            In_01.Text = depart_list.Count.ToString();
            for (int i = 0; i < depart_list.Count; i++)
            {
                In_03.Text += string.Format("" +
                    "<div class='search-result'>" +
                    "<h3><a href='{0}' target='_blank'>{1} - {2}</a></h3>" +
                    "<a href='{3}' target='_blank' class='search-link'>{4}</a>" +
                    "<div class='wrapper'><div class='accordion_wrap accordion_1'>" +
                    "<div class='accordion_header'>{5}</div>" +
                    "<div class='accordion_body'>" +
                    "<p>網站關鍵詞：{6}<br/><br/>" +
                    "熱門相關連結：<br/>{7}</p>" +
                    "</div></div></div>" +
                    "<div class='hr-line-dashed'></div>"
                    , web_list[i], depart_list[i], subpart_llist[i], web_list[i], web_list[i], sentence_list[i], show_keywords_list[i], show_search_list[i]);
            }

            //顯示時間
            sw.Stop(); //碼錶停止
            In_04.Text = sw.Elapsed.TotalSeconds.ToString();
        }

        
    }
}
