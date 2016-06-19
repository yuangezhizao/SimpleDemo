<%@ Page Title="" Language="C#" MasterPageFile="~/root.Master" AutoEventWireup="true" CodeBehind="RankingList.aspx.cs" Inherits="Webznm.news.RankingList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="/stylesheet/pavblog.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section id="sys-notification">
	<div class="container">
				<div id="notification"></div>
	</div>
</section>

    <section id="columns">
	<div id="breadcrumb"><ol class="breadcrumb container">
		<li class="first"><a href="<%=Domain %>"><span>首页</span></a></li>
		<li class="last"><a href="<%=Domain %>toplist" class="last"><span>排行榜</span></a></li>
	</ol></div><div class="container">
<div class="row">
			<aside class="col-lg-3 col-md-3 col-sm-12 col-xs-12">
			<div id="column-left" class="sidebar">
                <div class="box white latest_blog">
                    <div class="box-heading">
                        <span>最新资讯</span>
                    </div>
                    <div class="box-content">
                        <div class="pavblog-latest row">
                            <ul class="col-lg-4 col-md-4 col-sm-4">
                                 <li><a href="/Ranking/2014年奶粉品牌销量排行/1111"><i class="fa fa-bar-chart"></i> 2014年奶粉品牌销量排行</a></li>
                                  <li><a href="/Ranking/2014年奶粉品牌销量排行/1111"><i class="fa fa-bar-chart"></i> 2014年奶粉品牌销量排行</a></li>
                                  <li><a href="/Ranking/2014年奶粉品牌销量排行/1111"><i class="fa fa-square-o"></i> 2014年奶粉品牌销量排行</a></li>
                                  <li><a href="/Ranking/2014年奶粉品牌销量排行/1111"><i class="fa fa-square-o"></i> 2014年奶粉品牌销量排行</a></li>
                            </ul>


                        </div>
                    </div>
                </div>
			<div class="box pavblogs-comments-box">
	<div class="box-heading"><span>排行榜</span></div>
	<div class="box-content">
		<div class="clearfix">
						<div class="pavblog-comments">
								<div class="pav-comment media">
					<a class="pull-left" href="http://demopavothemes.com/pav_styleshop/d/index.php?route=pavblog/blog&amp;id=10#comment8" title="Test Test ">
						<img src="http://www.gravatar.com/avatar/08432a6bb1d51d71ffcffaa44e72b497?d=&amp;s=60" class="media-object img-responsive">
					</a>
					<div class="media-body">
						<p class="comment">nunc at In Curabitur mag Commodo laoreet semper tincidunt lorem Vestibulum nunc at In Curabitur mag</p>
						<span class="comment-author">Posted By Test Test ...</span>
					</div>					
				</div>
								<div class="pav-comment media">
					<a class="pull-left" href="http://demopavothemes.com/pav_styleshop/d/index.php?route=pavblog/blog&amp;id=10#comment7" title="ha cong tien">
						<img src="http://www.gravatar.com/avatar/60743b9b1e53dedb3b22977fdfaaf40e?d=&amp;s=60" class="media-object img-responsive">
					</a>
					<div class="media-body">
						<p class="comment">nunc at In Curabitur mag</p>
						<span class="comment-author">Posted By ha cong tien...</span>
					</div>					
				</div>
								<div class="pav-comment media">
					<a class="pull-left" href="http://demopavothemes.com/pav_styleshop/d/index.php?route=pavblog/blog&amp;id=10#comment6" title="ha cong tien">
						<img src="http://www.gravatar.com/avatar/60743b9b1e53dedb3b22977fdfaaf40e?d=&amp;s=60" class="media-object img-responsive">
					</a>
					<div class="media-body">
						<p class="comment">nunc at In Curabitur mag Commodo laoreet semper tincidunt lorem Vestibulum nunc at In Curabitur mag</p>
						<span class="comment-author">Posted By ha cong tien...</span>
					</div>					
				</div>
							</div>
					</div>
	</div>
</div>
	</div>		</aside>	
	 

	<section class="col-lg-9 col-md-9 col-sm-12 col-xs-12">		
		<div id="content">
			  
			<section class="pav-filter-blogs wrapper blog-wrapper">
				<div class="pav-blogs">
										<div class="leading-blogs blog-list-item">
						<div class="row">
														<div class="col-lg-12 col-sm-12 col-xs-12">
							<article class="blog-item">	
                                <header class="blog-header clearfix">	<h2 class="blog-title"><a href="http://demopavothemes.com/pav_styleshop/d/index.php?route=pavblog/blog&amp;id=12" title="Fashion Brunette Girl">2014年10奶粉品牌销售排行</a></h2></header>
	
				<section class="description">
			<p>据中心数据统计分析得出排名排名如下top1:贝因美，top2:贝因美，top3:贝因美。。。。</p>

                    
		</section>
	
		
	
	
	<footer>	

		<section class="blog-meta">
            	<span class="author">
				<span> <i class="fa fa-calendar"></i>: </span> 
				<span class="t-color">16:30</span>
			</span>
						<span class="author">
				<span><i class="fa fa-pencil"></i>: </span> 
				<span class="t-color">在哪买</span>
			</span>
            	

           
						
						<span class="publishin">
				<span title="支持"><i class="fa fa-thumbs-o-up"></i>: </span>
				<span class="t-color">0</span>
			</span>
						
						<span class="hits">
				<span title="反对"><i class="fa fa-insert-template"></i><i class="fa fa-thumbs-o-down"></i>: </span>
				<span class="t-color">275</span>
			</span>
						
						<span title="评论" class="comment_count">
				<span><i class="fa fa-comment-o"></i>: </span>
				<span class="t-color">0</span>
			</span>
           <span id="bdshare" class="bdshare_t bds_tools get-codes-bdshare feed-card-share" data="text:'苹果发布会何时也变得如此四平八稳？',url:'http://doc.sina.cn/?id=zl:27406',pic:'http://s.img.mix.sina.com.cn/auto/resize?img=http://upload.service.mix.sina.com.cn/255981acdf3a73546905d8a1b92cb5ef.jpg&amp;size=130_87'"><span class="bds_more">分享</span></span>
					</section>
	
			
	
	</footer>	

</article>	
                                                        
                                                            <article class="blog-item">	
                                <header class="blog-header clearfix">	<h2 class="blog-title"><a href="http://demopavothemes.com/pav_styleshop/d/index.php?route=pavblog/blog&amp;id=12" title="Fashion Brunette Girl">2014年10奶粉品牌销售排行</a></h2></header>
	
				<section class="description">
			<p>据中心数据统计分析得出排名排名如下top1:贝因美，top2:贝因美，top3:贝因美。。。。<a href="#">阅读全文</a> </p>

                    
		</section>
	
		
	
	
	<footer>	
        	
		<section class="blog-meta">
            <span class="author">
				<span> <i class="fa fa-calendar"></i>日期：</span><span class="t-color">16:30</span>
			</span>
						<span class="author">
				<span><i class="fa fa-tags"></i>标签: </span> 
				<span class="t-color"><a href="#">母婴</a>,<a href="#">母婴</a></span>
			</span>
				<%--		
						<span class="publishin">
				<span title="支持"><i class="fa fa-thumbs-o-up"></i>: </span>
				<span class="t-color">0</span>
			</span>
						
						<span class="hits">
				<span title="反对"><i class="fa fa-insert-template"></i><i class="fa fa-thumbs-o-down"></i>: </span>
				<span class="t-color">275</span>
			</span>
						
						<span title="评论" class="comment_count">
				<span><i class="fa fa-comment-o"></i>: </span>
				<span class="t-color">0</span>
			</span>

            		<span class="publishin">
				<span title="支持"><i class="fa fa-share-alt"></i>: </span>
				<span class="t-color">0</span>
			</span>
                    --%>
            
           <span id="bdshare" class="bdshare_t bds_tools get-codes-bdshare feed-card-share" data="text:'苹果发布会何时也变得如此四平八稳？',url:'http://doc.sina.cn/?id=zl:27406',pic:'http://s.img.mix.sina.com.cn/auto/resize?img=http://upload.service.mix.sina.com.cn/255981acdf3a73546905d8a1b92cb5ef.jpg&amp;size=130_87'"><span class="bds_more"><i class="fa fa-share-alt"></i>分享</span></span>
					</section>
	
			
	
	</footer>	

</article>





														</div>
															
																				</div>
					</div>
					
										
									
					<div class="pav-pagination clearfix"><div class="results">Showing 1 to 5 of 5 (1 Pages)</div></div>
				</div>
			</section>
					</div>
	</section> 

		
</div></div>	

</section>







<script type="text/javascript" id="bdshare_js" data="type=tools" ></script> 
<script type="text/javascript" id="bdshell_js"></script> 
      <style type="text/css">
#bdshare{float:none !important;font-size:13px !important;padding-bottom:0 !important;}
#bdshare span.bds_more{display:inline !important;padding:0 !important;float:none !important;font-family:"Microsoft YaHei","微软雅黑" !important;background-image:none !important;}
#bdshare span.bds_more:hover{color:#228fdd;}
</style>
<script type="text/javascript">
    document.getElementById('bdshell_js').src = "http://bdimg.share.baidu.com/static/js/shell_v2.js?cdnversion=" + Math.ceil(new Date() / 3600000);
</script>
<!-- Baidu Button END -->
	
</asp:Content>


