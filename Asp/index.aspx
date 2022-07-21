<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="E_Final_KeyShow.index" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>畢業專題</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.5.0/css/font-awesome.min.css" />
    <link href="RES/css/search_main.css" rel="stylesheet" type="text/css" />
    <link href="RES/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js"></script>
    <script type="text/javascript">
        $(function () {
            /* 按下GoTop按鈕時的事件 */
            $('#gotop').click(function () {
                $('html,body').animate({ scrollTop: 0 }, 'slow');   /* 返回到最頂上 */
                return false;
            });
            /* 偵測卷軸滑動時，往下滑超過400px就讓GoTop按鈕出現 */
            $(window).scroll(function () {
                if ($(this).scrollTop() > 200) {
                    $('#gotop').fadeIn();
                } else {
                    $('#gotop').fadeOut();
                }
            });
            /* 展開更多敘述 */
            $(".accordion_header").click(function () {
                $(".accordion_header").removeClass("active");
                $(this).addClass("active");
            });

            $(".accordion_body").click(function () {
                $(".accordion_header").removeClass("active");
            });
        });
    </script>
</head>

<body>
    <form name="userSearch" method="post" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True"></asp:ScriptManager>
        <!-- =============================================================================================================== -->
        <div class="sidebar-container">
            <div class="sidebar-logo">熱門網站列表</div>
            <ul class="sidebar-navigation">
                <asp:Repeater runat="server" ID="rpt_sub_list" OnItemDataBound="rpt_sub_list_ItemDataBound">
                    <ItemTemplate>
                        <asp:Button ID="btn_sub_list" runat="server" CssClass="bn632-hover bn26" OnClick="sub_list_click" />
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
        <div class="container bootstrap snippets bootdey">
            <div class="row">
                <div class="col-lg-12">
                    <div class="ibox float-e-margins">
                        <div class="ibox-content">
                            <!-- =============================================================================================================== -->
                            <div class="search-form">
                                <h1 style="text-align: center">中華大學 校園官網 快速搜尋</h1>

                                <div class="input-group">
                                    <!-- =============================================================================================================== -->
                                    <asp:TextBox CssClass="form-control input-lg" ID="TextBox1" runat="server"
                                        onfocus="this.value=''" value="資工系" placeholder="以 空格 區分關鍵詞"
                                        AutoPostBack="True"></asp:TextBox>
                                    <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender1" runat="server"
                                        TargetControlID="TextBox1"
                                        ServiceMethod="GetKeywords"
                                        EnableCaching="true"
                                        MinimumPrefixLength="1"
                                        CompletionSetCount="1"
                                        CompletionInterval="10"
                                        CompletionListCssClass="completionList"
                                        CompletionListHighlightedItemCssClass="itemHighlighted"
                                        CompletionListItemCssClass="listItem">
                                    </ajaxToolkit:AutoCompleteExtender>
                                    <!-- =============================================================================================================== -->
                                    <div class="input-group-btn">
                                        <asp:Button CssClass="btn btn-lg btn-primary" ID="Button1" runat="server" Text="搜尋" OnClick="SearchBar" />
                                    </div>
                                </div>

                                <small>資料庫數據總量為
                                    <asp:Label ID="In_00" runat="server" Text=""></asp:Label>
                                    項。</small>

                                <asp:Label ID="test" runat="server" ViewStateMode="Enabled"></asp:Label>

                                <div class="input-group">
                                    <div class="input-group-btn" style="text-align: center;">
                                        <asp:Button CssClass="btn btn-lg btn-primary" Style="margin-right: 50px;" ID="acad_btn" runat="server" Text="學術單位" OnClick="unityBtn" />
                                        <asp:Button CssClass="btn btn-lg btn-primary" ID="offic_btn" runat="server" Text="行政單位" OnClick="unityBtn" />
                                        <asp:Button CssClass="btn btn-lg btn-primary" Style="margin-left: 50px;" ID="other_btn" runat="server" Text="其他連結" OnClick="unityBtn" />
                                    </div>
                                </div>
                            </div>
                            <!-- =============================================================================================================== -->
                            <asp:Panel ID="Panel1" runat="server" Visible="False">
                                <h2 style="font-weight: bold;">總共
                                    <asp:Label ID="In_01" runat="server" Text=""></asp:Label>
                                    項符合結果，搜尋到
                                    <span class="text-navy">「<asp:Label ID="In_02" runat="server" Text="Label"></asp:Label>」</span>
                                </h2>
                                <br />

                                <div class="sidebar-right">
                                    <div class="sidebar-logo">分支</div>
                                    <ul class="sidebar-navigation">
                                        <asp:Repeater runat="server" ID="rpt_SelectNext" OnItemDataBound="rpt_SelectNext_ItemDataBound">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chk_SelectNext" OnCheckedChanged="chk_SelectNext_CheckedChanged" AutoPostBack="true" Font-Size="18px" ForeColor="#070b46" />
                                                <br />
                                            </ItemTemplate>
                                    </asp:Repeater>
                                    </ul>
                                    
                                </div>

                                <small>搜尋耗時(
                                    <asp:Label ID="In_04" runat="server" Text=""></asp:Label>
                                    秒)</small>
                                <br />
                                <!-- =============================================================================================================== -->
                                <asp:PlaceHolder ID="PlaceHolder_01" runat="server"></asp:PlaceHolder>
                                <div class="hr-line-dashed"></div>
                                <div class="search-result">
                                    <asp:Label ID="In_03" runat="server" class="search-result" ViewStateMode="Disabled"></asp:Label>
                                </div>
                            </asp:Panel>
                            <!-- =============================================================================================================== -->
                            <br />
                            <asp:Panel ID="Panel2" runat="server" Visible="False">
                                <div>
                                    <h1>學術單位</h1>

                                    <article class="episode">
                                        <div class="episode__content">
                                            <div class="title">資訊電機學院</div>
                                            <div class="story">
                                                <asp:Label ID="In_unity_0" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>
                                    </article>

                                    <article class="episode">
                                        <div class="episode__content">
                                            <div class="title">管理學院</div>
                                            <div class="story">
                                                <asp:Label ID="In_unity_1" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>
                                    </article>

                                    <article class="episode">
                                        <div class="episode__content">
                                            <div class="title">建築與設計學院</div>
                                            <div class="story">
                                                <asp:Label ID="In_unity_2" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>
                                    </article>

                                    <article class="episode">
                                        <div class="episode__content">
                                            <div class="title">人文社會學院</div>
                                            <div class="story">
                                                <asp:Label ID="In_unity_3" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>
                                    </article>

                                    <article class="episode">
                                        <div class="episode__content">
                                            <div class="title">觀光學院</div>
                                            <div class="story">
                                                <asp:Label ID="In_unity_4" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>
                                    </article>

                                    <article class="episode">
                                        <div class="episode__content">
                                            <div class="title">創新產業學院</div>
                                            <div class="story">
                                                <asp:Label ID="In_unity_5" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>
                                    </article>

                                    <article class="episode">
                                        <div class="episode__content">
                                            <div class="title">通識教育中心</div>
                                            <div class="story">
                                                <asp:Label ID="In_unity_6" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>
                                    </article>

                                </div>
                            </asp:Panel>
                            <!-- =============================================================================================================== -->
                            <br />
                            <asp:Panel ID="Panel3" runat="server" Visible="False">
                                <div>
                                    <h1>行政單位</h1>

                                    <article class="episode">
                                        <div class="episode__content">
                                            <div class="title">各單位處室</div>
                                            <div class="story">
                                                <asp:Label ID="In_unity_7" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>
                                    </article>
                                </div>
                            </asp:Panel>
                            <!-- =============================================================================================================== -->
                            <br />
                            <asp:Panel ID="Panel4" runat="server" Visible="False">
                                <div>
                                    <h1>其他連結</h1>

                                    <article class="episode">
                                        <div class="episode__content">
                                            <div class="title">其他</div>
                                            <div class="story">
                                                <asp:Label ID="In_unity_8" runat="server" Text=""></asp:Label>
                                            </div>
                                        </div>
                                    </article>
                                </div>
                            </asp:Panel>
                            <!-- =============================================================================================================== -->
                            <a id="refresh" class="redrict" href="javascript:window.location.replace(window.location.href)"><i class="fa fa-refresh my-float"></i></a>
                            <a id="gotop" class="gotop"><i class="fa fa-angle-up my-float"></i></a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
