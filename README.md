# MEDIC Visualizer

<div class="wikidoc">
<p>The MARC-HI Visualizer is a utility that visualizes a series of RFC3881 (ATNA) messages by interpreting the audit messages and broadcasting events to clients who represent the traffic in XAML diagrams.
</p>
<p>The architecture of the Visualizer is as follows: </p>
<p><img src="http://te.marc-hi.ca/projects/VIS/architecture.png"></p>
</div><div class="ClearBoth"></div>

# Configuring the Visualizer

<div class="wikidoc">
<p>Modifying the configuration for the Visualizer Service is quite simple. There are several pieces of configuration information that are needed:</p>
<ol>
<li>
<p>The address/port that the SysLog listening service listens to</p>
</li><li>
<p>The port of the notification broadcast service</p>
</li><li>
<p>Whether or not the CAP service should be enabled (this is only true if Silverlight clients are connecting)</p>
</li></ol>
<p>All configuration for the Visualization Service is contained in the MARC.EHRS.VisualizationServer.exe.config file in the installation directory. Using your favorite text editor, open this file.</p>
<h4>Configuring the SysLog Listener</h4>
<p>First, we have to ensure that the visualizationserver configuration sections are registered (or else the configuration reader won't load your configuration). Ensure that the following lines exist in the configuration file:</p>
<pre>&lt;section name=&quot;marc.ehrs.visualizationserver.syslog&quot; <br>  type=&quot;MARC.EHRS.VisualizationServer.Syslog.Configuration.ConfigurationHandler,<br>                         MARC.EHRS.VisualizationServer.Syslog, Version=1.0.0.0&quot;/&gt;<br>           &lt;section name=&quot;marc.ehrs.visualizationserver.notifier&quot;<br>              type=&quot;MARC.EHRS.VisualizationServer.Notifier.Configuration.ConfigurationHandler,<br>                         MARC.EHRS.VisualizationServer.Notifier, Version=1.0.0.0&quot;/&gt;</pre>
<p>Next, locate the SysLog configuration and set the listening address and port, by default the Visualization Service will listen on 127.0.0.1 on port 11514. To change this, modify this configuration setting:</p>
<pre> &lt;marc.ehrs.visualizationserver.syslog maxMessageSize=&quot;65536&quot;&gt; <br>&nbsp;&nbsp;&nbsp; &lt;binding address=&quot;127.0.0.1&quot;<br>&nbsp;&nbsp;&nbsp; port=&quot;11514&quot;/&gt;<br> &lt;/marc.ehrs.visualizationserver.syslog&gt;<br>
</pre>
<p>You can add aditional bindings (so the visualizer can listen on different ports/ip addresses as well), and you may optionally specify a forwardAddress and forwardPort which will instruct the visualizer to forward received messages received on the specified
 port to the appropriate address.</p>
<p>After you've applied these settings, you will need to restart the Visualization Service host. Using the command prompt type the following command:</p>
<pre>net stop &quot;Visualization Service&quot;<br>net start &quot;Visualization Service&quot;</pre>
<h4>Configuring the Notifier</h4>
<p>The notifier is a piece of code that clients connect to in order to receive events. The notifier service is essential for clients to receive the necessary messages to &quot;light up&quot; the visualizer diagrams.</p>
<p>The notifier is configured using the following parameters:</p>
<ul>
<li>
<p>port - The port that the notifier will broadcast events on and will use to accept connections</p>
</li><li>
<p>enableCAPServer - When set to true, the notifier will serve as a ClientAccessPolicy provider.</p>
</li><li>
<p>capServerPolicyFile - Identifies the ClientAccessPolicy file to send to Silverlight clients attempting to establish connections.</p>
</li></ul>
<p>The structure of this configuration section is as follows:</p>
<pre>&lt;marc.ehrs.visualizationserver.notifier<br>   port=&quot;4530&quot;<br>   enableCAPServer=&quot;true&quot;<br>   capServerPolicyFile=&quot;ClientAccessPolicy.xml&quot;/&gt;</pre>
</div><div class="ClearBoth"></div>

# Configuring the client

<div class="wikidoc">
<p>By default the Visualization Client that is distributed with the Visualizer comes with no diagrams or configuration. When you launch the Visualization Client and click on either &quot;Change Diagram&quot; or &quot;Connect&quot; you will be presented with an empty option list
 (like below)</p>
<p><img src="http://te.marc-hi.ca/projects/VIS/ChangeDiagram.png" alt=""></p>
<p>In order to get the visualizer to load diagrams and connect to a server, you will need to deploy the
<strong>WEB</strong> folder (in the install directory) to a web server. Doing this is outside of the scope of this documentation, I will assume you've deployed it to localhost:8008</p>
<p>Our first task, is to instruct the Visualizer Client to load a configuration file, this is done by editing the default.html file (or the approporiate &lt;object...&gt; tag in your source code).</p>
<p>Make sure the following <strong>initparams</strong> are specified:</p>
<pre> &lt;object data=&quot;data:application/x-silverlight-2,&quot; type=&quot;application/x-silverlight-2&quot; width=&quot;100%&quot; height=&quot;100%&quot;&gt;
             &lt;param name=&quot;source&quot; value=&quot;ClientBin/MARC.EHRS.VisualizationClient.Silverlight.xap&quot;/&gt;
         &lt;param name=&quot;initparams&quot; value=&quot;<strong>config=http://localhost:8080/visualizer.xml</strong>&quot; /&gt; ...</pre>
<p>Where <strong>config=</strong> points to the appropriate configuration file. Next, you'll have to ensure that the configuration file exists. Using your favourite text editor, create the file specified in your initparams. Create an empty file that looks like
 this:</p>
<pre> &lt;configuration xmlns:xaml=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;&gt;
         &lt;/configuration&gt;</pre>
<p>This represents an empty configuration file that the visualization client will read. Next, we need to register diagrams with the configuration file. To do this, add a
<strong>diagram</strong> element to the configuration section. If you're using the sample XAML file available from the downloads seciton, your config file would look similar to the following:</p>
<pre> &lt;configuration xmlns:xaml=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;&gt;
            &lt;diagram src=&quot;http://localhost:8080/SAMPLE.XAML&quot; name=&quot;A Sample Diagram&quot;/&gt;
         &lt;/configuration&gt;</pre>
<p>If you visit http://localhost:8080/default.html and click &quot;Change Diagram&quot; you will be presented with the contents of your config file. The following example illustrates my demo environment:</p>
<p><img src="http://te.marc-hi.ca/projects/VIS/ChangeDiagram2.png" alt=""></p>
<p>If you select the sample diagram your visualizer client should display the empty diagram. Next we need to let the client know what visualization servers are available in the Connect dialog. To do this, add
<strong>server</strong> elements to your diagram. If you're attempting to connect to a local server you can use the following exmaple file:</p>
<pre> &lt;configuration xmlns:xaml=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;&gt;
            &lt;diagram src=&quot;http://localhost:8080/SAMPLE.XAML&quot; name=&quot;A Sample Diagram&quot;/&gt;
            &lt;server address=&quot;127.0.0.1&quot; port=&quot;4530&quot; name=&quot;Local Server&quot; imageSrc=&quot;http://localhost:8080/sampleServer.png&quot;/&gt;
         &lt;/configuration&gt;</pre>
<p>The server element contains more attributes than the diagram element. The <strong>
address</strong> attribute instructs the client the IP address or host name that is running the visualization service. The
<strong>port</strong> attribute specifies the port that notifications are broadcast on. The
<strong>imageSrc</strong> is a graphical representation of the server. If you have a logo (or many different servers) this can help the user distinguish between them</p>
<p>Finally, you can specify some data inside of the &quot;about&quot; dialog by simply using XAML markup within the root element. For example, if I wanted a paragraph that explains my product in the visualizer (for clients to read) you can simply use the following configuration:</p>
<pre> &lt;configuration xmlns:xaml=&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation&quot;&gt;
            &lt;diagram src=&quot;http://localhost:8080/SAMPLE.XAML&quot; name=&quot;A Sample Diagram&quot;/&gt;
            &lt;server address=&quot;127.0.0.1&quot; port=&quot;4530&quot; name=&quot;Local Server&quot; imageSrc=&quot;http://localhost:8080/sampleServer.png&quot;/&gt;
            &lt;xaml:Paragraph&gt;
                &lt;xaml:Run&gt;Product A communicates with Product B using IHE PIX/PDQv3, Product A is distributed by&lt;/xaml:Run&gt;
                &lt;xaml:Hyperlink NavigateUri=&quot;http://www.mohawkcollege.ca&quot;&gt;Mohawk College&lt;/xaml:Hyperlink&gt;
           &lt;/xaml:Paragraph&gt;
        &lt;/configuration&gt;</pre>
</div><div class="ClearBoth"></div>

# Configuring a client access policy file

<div class="wikidoc">
<p>By default, Silverlight applications will not be able to connect to the Visualization Service unless an appropriate ClientAccessPolicy file is served to them. Whenever the Silverlight application attempts to connect to a Visualization Service instance, the
 Silverlight framework will attempt to download a Client Access Policy file from port 943 of the server (for example, if the client is connecting to 127.0.0.1:4530 then Silverlight will attempt to download a client access policy file from 127.0.0.1:943)</p>
<p>The client access policy file dictates which Silverlight clients are allowed to connect to the visualization service. The default ClientAccessPolicy.xml file included with the Visualization Service allows all TCP connections to port 4530 for any client.
 You can modify this file to restrict access to clients.</p>
</div><div class="ClearBoth"></div>
