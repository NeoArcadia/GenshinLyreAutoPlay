using Commons.Music.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenshinLyreAutoPlay
{
    public partial class Form1 : Form
    {

        private Dictionary<string, int> letter = new Dictionary<string, int>
        {
            { "a", 65},{ "b", 66},{ "c", 67},{ "d", 68},{ "e", 69},{ "f", 70},{ "g", 71},{ "h", 72},{ "i", 73},{ "j", 74},{ "k", 75},{ "l", 76},{ "m", 77},{ "n", 78},{ "o", 79},{ "p", 80},{ "q", 81},{ "r", 82},{ "s", 83},{ "t", 84},{ "u", 85},{ "v", 86},{ "w", 87},{ "x", 88},{ "y", 89},{ "z", 90 }
        };

        private Dictionary<string, string> mapping =new Dictionary<string, string>
         { 
            {"48", "z"},{ "50", "x"},{ "52", "c"},{ "53", "v"},{ "55", "b"},{ "57", "n"},{ "59", "m"},{ "60", "a"},{ "62", "s"},{ "64", "d"},{ "65", "f"},{ "67", "g"},{ "69", "h"},{ "71", "j"},{ "72", "q"},{ "74", "w"},{ "76", "e"},{ "77", "r"},{ "79", "t"},{ "81", "y"},{ "83", "u"}
         };


        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles("songs", "*.mid");
            foreach (string fn in files)
            {
                comboBox1.Items.Add(fn);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //FindWindow 参数一是进程名 参数2是 标题名
            IntPtr calculatorHandle = WinApiUtils.FindWindow(null, "原神");
            //判断是否找到
            if (calculatorHandle == IntPtr.Zero)
            {
                MessageBox.Show("没有找到!");
                return;
            }
            //var access = MidiAccessManager.Default;
            var music = MidiMusic.Read(System.IO.File.OpenRead("./" + comboBox1.SelectedItem));
            SimpleAdjustingMidiPlayerTimeManager simpleAdjustingMidiPlayerTimeManager = new SimpleAdjustingMidiPlayerTimeManager();
            var player = new MidiPlayer(music, simpleAdjustingMidiPlayerTimeManager);
            int val = 0;
            string key;
            WinApiUtils.SetForegroundWindow(calculatorHandle);
            player.EventReceived += (MidiEvent me) =>
            {
                switch (me.EventType)
                {
                    case MidiEvent.NoteOn:
                        val = me.Msb;
                        key = val.ToString();
                        if (mapping.ContainsKey(key))
                        {
                            //SendKeys.Send(mapping[key]);
                            WinApiUtils.keybd_event((byte)letter[mapping[key]], 0, 0, 0);
                            //WinApiUtils.PostMessage(calculatorHandle, WinApiUtils.WM_KEY_DOWN, (Keys)Enum.Parse(typeof(Keys), mapping[key].ToUpper()), letter[mapping[key]]);
                            textBox1.Text += mapping[key];
                            textBox1.Text += ";";
                        }
                        textBox1.Text += "\r\n";
                        break;
                    case MidiEvent.NoteOff:
                        val = me.Msb;
                        key = val.ToString();
                        if (mapping.ContainsKey(key))
                        {
                            //SendKeys.Send(mapping[key]);
                            //WinApiUtils.PostMessage(calculatorHandle, WinApiUtils.WM_KEY_UP, (Keys)Enum.Parse(typeof(Keys), mapping[key].ToUpper()), letter[mapping[key]]);
                            WinApiUtils.keybd_event((byte)letter[mapping[key]], 0, 2, 0);
                        }
                        break;
                    default:
                        //textBox1.Text += me.ToString();
                        //textBox1.Text += "\r\n";
                        break;

                }

            };
            player.Play();
        }
    }
}
