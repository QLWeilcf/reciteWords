using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

/*一个背单词的小程序；
 * 将英语四级单词文本文件的内容读出来及放到内存的数组或列表中->在第38行的readFile()函数，用ArrayList去装数据；用到了正则和异常处理。
 *使用Timer,每隔一定时间，让英语单词及汉语意思显示到屏幕上（可以用两个标签控件）-> 核心部分在oneSecond_Tick()和timer1_Tick()；对应122和119行
 *加点花样，如随机，可调整背单词速度->本程序的出词是随机的，而且不会重复；点击 设置时间间隔 可以调整背单词速度；还加入了改变词汇的展示颜色、超链接到谷歌翻译、有提示时间等功能，在时间少于5秒时，timeLabl的背景颜色会改变。
 * 支持打开自己的词汇文件（要求格式和老师的相同）
 * 其他功能欢迎向我建议。（2017-4-15 20:46:46）
 * 参考资料：线程间操作 http://blog.sina.com.cn/s/blog_68ed8b2101018fqm.html
 * timer控件：唐老师的文本 http://www.icourse163.org/learn/PKU-1001663016?tid=1002052006#/learn/content?type=detail&id=1002688132&cid=1002966430
 * */

namespace reciteWords {
    public partial class Form1 : Form {
        public ArrayList allWords = new ArrayList();
        public int interval = 20;// 默认时间间隔
        public int user =20;//用于自定义时间间隔
        public string curDir = Directory.GetCurrentDirectory() + "\\College_Grade4.txt";//词汇文件目录
        
        public Form1 () {
            MessageBox.Show("欢迎使用本软件；\n开始背单词之旅吧！","欢迎！！！");
            InitializeComponent();
            displayWords();//主程序部分
        }
        
        public bool readFile (string curDir) {//读取文件
            //输入的curDir为词汇文件目录
            StreamReader sr = new StreamReader(curDir,Encoding.GetEncoding("gb2312"));//如果打开自定文件这里出错请用Encoding.UTF8
            Regex rgx = new Regex(@"\t");//正则去切分词汇
            try {//使用异常处理
                while (!sr.EndOfStream) {
                    string oneLine = sr.ReadLine();
                    string[] words = rgx.Split(oneLine);
                    ArrayList wordGup = new ArrayList();
                    string[] wgup = { words[1], words[2], "0" };
                    this.allWords.Add(wgup);
                }
                sr.Close();
                return true;
            }catch(Exception e) {
                MessageBox.Show("读取失败" + e.Message,"提示");
                return false;
            }
        }

        public void displayWords () {
            Control.CheckForIllegalCrossThreadCalls = false;//处理线程问题
            bool a = readFile(this.curDir);
            while (!a) {//用while更好
                openAgain();
                a=readFile(this.curDir);
            }
            showWords();//选词函数
            timer1.Enabled=true;
        }
        
        public void showWords () {//每次随机选词
            Random ran = new Random();
            int lineLen = this.allWords.Count;
            int chiose = ran.Next(0, lineLen);
            string[] oneCho =(string[]) this.allWords[chiose];
            if (oneCho[2] == "0") {
                englishLbl.Text = oneCho[0];
                chineseLbl.Text = oneCho[1];
                oneCho[2] += 1; //字符串的加和。
            }
            else { //
                showWords();
            }
        }

        public void openAgain () {
            openFileDialog1.Title = "打开词汇文件";
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog1.Filter = "文本文件（*.txt）|*.txt";
            openFileDialog1.FileName = "College_Grade4";
            if (openFileDialog1.ShowDialog() == DialogResult.OK) 
                this.curDir = openFileDialog1.FileName;
        }

        private void 随机背词ToolStripMenuItem_Click (object sender, EventArgs e) {
            showWords();
        }
        private void 打开OToolStripMenuItem_Click (object sender, EventArgs e) {//用于打开词汇文件
            openAgain();
        }

        private void 搜索SToolStripMenuItem_Click (object sender, EventArgs e) {//可以搜索你想要的词
            System.Diagnostics.Process.Start("https://translate.google.cn");
        }

        private void 关于AToolStripMenuItem_Click (object sender, EventArgs e) {
            MessageBox.Show("背单词软件；版本1.1；编写者lcf");
        }
        private void 翻译TToolStripMenuItem_Click (object sender, EventArgs e) {
            System.Diagnostics.Process.Start("https://translate.google.cn");
        }
        
        private void Form1_Load (object sender, EventArgs e) {
            IntvInput.Visible = false;
            timer1.Enabled = true;
            oneSecond.Start();
            timer1.Start();
            timer1.Interval = this.user * 1000;
        }

        private void timer1_Tick (object sender, EventArgs e) {//这个timer用于改变词汇
            showWords();
        }
        private void oneSecond_Tick (object sender, EventArgs e) {
            timeLabl.Text = this.interval.ToString() + " s";
            this.interval--;
            if (this.interval == -1) {
                this.user = Convert.ToInt32(IntvInput.Text);
                this.interval = this.user;//默认是20s
                IntvInput.Enabled = false;
            }
            if (this.interval < 5) 
                timeLabl.BackColor = Color.Red;
            else//背景色的小变换
                timeLabl.BackColor = Color.WhiteSmoke;
        }

        private void 调节时间间隔JToolStripMenuItem_Click (object sender, EventArgs e) {
            IntvInput.Visible = true;
            IntvInput.Enabled = true;
            try {
                this.user = Convert.ToInt32(IntvInput.Text);
                timer1.Interval = this.user * 1000;
            }
            catch (Exception exc) {
                MessageBox.Show("请输入数字" + exc.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void 设置文本颜色ToolStripMenuItem_Click (object sender, EventArgs e) {
            if (colorDialog1.ShowDialog() == DialogResult.OK) {
                englishLbl.ForeColor = colorDialog1.Color;
                chineseLbl.ForeColor = colorDialog1.Color;
            }
        }
        private void 保存SToolStripMenuItem_Click (object sender, EventArgs e) {
            ToBeDeveloped();
        }

        private void 另存为AToolStripMenuItem_Click (object sender, EventArgs e) {
            ToBeDeveloped();
        }

        private void 打印PToolStripMenuItem_Click (object sender, EventArgs e) {
            ToBeDeveloped();
        }
        private void 自定义CToolStripMenuItem_Click (object sender, EventArgs e) {
            ToBeDeveloped();

        }

        public void ToBeDeveloped () {
            MessageBox.Show("待开发，请体验其他有趣的功能吧！", "提示");
        }

        private void 退出XToolStripMenuItem_Click (object sender, EventArgs e) {
            Application.Exit();
        }

    }

}
