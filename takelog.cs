using System;
using System.Windows.Forms;
using System.IO;


//Copyright (c) 2008-2018 David Tinney
//
//Permission to use, copy, modify, and/or distribute this software for any purpose with or without fee
// is hereby granted, provided that the above copyright notice and this permission notice appear in all copies.
//
//THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT, INDIRECT,
// OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE,
// DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION,
// ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.

static class program
{
 [STAThread]
 static void Main()
 {
  Application.Run(new timelog());
 }
}
class timelog : Form
{
    int PopupInterval = 30;
 string logfile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Timelog.txt";
 Timer UpdateTimeTimer;
 Timer PopupTimer;

 public timelog()
 {
  SuspendLayout(); //don't paint the form as we go, wait until we're finished building the form
  AutoScaleDimensions = new System.Drawing.SizeF(6, 13);
  AutoScaleMode = AutoScaleMode.Font;
  MaximizeBox = false;
  Name = "Timelog";
  Text = "Log Time!";
  PopupTimer = new Timer(){Enabled = false,Interval = PopupInterval * 60000};
  UpdateTimeTimer = new Timer(){Enabled = false,Interval = 5};
  PopupTimer.Tick += (s,e)=>{dotick();};
  UpdateTimeTimer.Tick += (s,e)=>{uptime();};
  Controls.Add(new TextBox()
    {Location = new System.Drawing.Point(137, 2) ,Name = "LogText"  ,Size = new System.Drawing.Size(570, 20),TabIndex = 0,Text = "",ScrollBars = ScrollBars.Vertical});
  Controls.Add(new Label()
    {Location = new System.Drawing.Point(12, 9)  ,Name = "TimeLabel",Size = new System.Drawing.Size(119, 13),TabIndex = 3,Text = "HH:MM MM/DD/YYYY",AutoSize = true});
  Controls.Add(new Label()
    {Location = new System.Drawing.Point(275, 40),Name = "LastLog"  ,Size = new System.Drawing.Size(450, 26),TabIndex = 3,Text = lastlogline(),AutoSize = false});
  Controls.Add(new Button()
    {Location = new System.Drawing.Point(15, 28) ,Name = "LogButton",Size = new System.Drawing.Size(119, 28),TabIndex = 1,Text = "Log",UseVisualStyleBackColor = true});
  Controls.Add(new Button()
    {Location = new System.Drawing.Point(144, 28),Name = "Quit"     ,Size = new System.Drawing.Size(117, 28),TabIndex = 2,Text = "Stop Logging",DialogResult = DialogResult.Cancel,UseVisualStyleBackColor = true});
  Controls.Add( new Button()
    {Location = new System.Drawing.Point(728, 2), Name = "m15"      ,Size = new System.Drawing.Size(32, 18) ,TabIndex = 4,Text = "15",UseVisualStyleBackColor = true});
  Controls.Add( new Button()
    {Location = new System.Drawing.Point(728, 22),Name = "m30"      ,Size = new System.Drawing.Size(32, 18) ,TabIndex = 4,Text = "30",UseVisualStyleBackColor = true});
  Controls.Add( new Button()
    {Location = new System.Drawing.Point(728, 42),Name = "m60"      ,Size = new System.Drawing.Size(32, 18) ,TabIndex = 4,Text = "60",UseVisualStyleBackColor = true});

  getcontrol("LogButton").Click += (s,e)=>{if (getcontrol("LogText").Text!=""){log();}PopupTimer.Enabled=true;WindowState=FormWindowState.Minimized;};
  getcontrol("Quit").Click += (s,e)=>{if(getcontrol("LogText").Text!=""){log();}Close();Application.Exit();};
  getcontrol("m15").Click += (s,e)=>{minchange(15);};
  getcontrol("m30").Click += (s,e)=>{minchange(30);};
  getcontrol("m60").Click += (s,e)=>{minchange(60);};
  AcceptButton = (IButtonControl)getcontrol("LogButton");
  CancelButton = (IButtonControl)getcontrol("Quit");
  Load += (s,e)=>{dotick();};
  Resize += (s,e)=>{if(WindowState!=FormWindowState.Minimized){dotick();}else{UpdateTimeTimer.Enabled=false;}};
  ClientSize = new System.Drawing.Size(762, 71);
  minchange(30);
  ResumeLayout(false);
  PerformLayout();
 }

 Control getcontrol(string name){return Controls.Find(name, true)[0];}

 void dotick()
 {
  uptime();
  UpdateTimeTimer.Interval = 300000;
  UpdateTimeTimer.Enabled = true;
  getcontrol("LastLog").Text = lastlogline();
  WindowState = FormWindowState.Normal;
  Activate();
  PopupTimer.Enabled = false;
 }

 void log()
 {
  System.IO.StreamWriter f;
  string thistime = getcontrol("TimeLabel").Text;
  thistime += new string(' ', 25 - thistime.Length);
  if (System.IO.File.GetLastWriteTime(logfile).Day != System.DateTime.Today.Day)
    {thistime = "\r\n" + thistime;}
  f = new StreamWriter(logfile, true);
  f.WriteLine(thistime + getcontrol("LogText").Text);
  getcontrol("LogText").Text="";
  f.Close();
 }

 void uptime()
 {
  Activate();
  getcontrol("TimeLabel").Text = System.DateTime.Now.ToString("HH:mm  MM/dd/yyyy");
 }

 string lastlogline()
 {
  StreamReader f;
  string thisstr = "";
  string ret = "";
  //open the logfile, create it if not exists
  if (!System.IO.File.Exists(logfile)){
    var t= new System.IO.StreamWriter(logfile);
    t.WriteLine("");
    t.Close();
  }
  f = new System.IO.StreamReader(logfile);
  while (!f.EndOfStream)
  {
   thisstr = f.ReadLine();
   if (thisstr != ""){ret = thisstr;}
  }
  f.Close();
  return (ret);
 }

 void minchange(int minutes)
 {
  PopupInterval = minutes;
  getcontrol("m15").Enabled = !(minutes==15);
  getcontrol("m30").Enabled = !(minutes==30);
  getcontrol("m60").Enabled = !(minutes==60);
  PopupTimer.Interval =  minutes * 60000;
 }
}
