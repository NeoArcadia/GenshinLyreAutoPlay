using Commons.Music.Midi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

        private MidiPlayer player;

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

            comboBox1.SelectedIndexChanged += (o, args) =>
            {
                button1.Enabled = true;
                button2.Enabled = true;
            };
            button1.Enabled = false;
            button2.Enabled = false;

            var midiInput = new MidiInput();
            midiInput.Start(cb_selectMidiDev);
            midiInput.midiInputEvent += sendKeyWithoutShift;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Thread.Sleep(5000);
            //FindWindow 参数一是进程名 参数2是 标题名
            textBox1.Text = "";
            listBox1.Items.Clear();
            int shift = 0;
            IntPtr calculatorHandle = WinApiUtils.FindWindow(null, "原神");
//            IntPtr calculatorHandle = WinApiUtils.FindWindow(null, "untitled.txt - 记事本");
            //判断是否找到
            if (calculatorHandle == IntPtr.Zero)
            {
                MessageBox.Show("没有找到!");
                return;
            }
            //var access = MidiAccessManager.Default;
            var music = MidiMusic.Read(System.IO.File.OpenRead("./" + comboBox1.SelectedItem));
            shift = TuneUtils.getBestShift(music);
            textBox1.Text += "best shift =" + shift;
            textBox1.Text += "\r\n";
            SimpleAdjustingMidiPlayerTimeManager simpleAdjustingMidiPlayerTimeManager = new SimpleAdjustingMidiPlayerTimeManager();
            player = new MidiPlayer(music, simpleAdjustingMidiPlayerTimeManager);

            WinApiUtils.SetForegroundWindow(calculatorHandle);
            player.EventReceived += (MidiEvent me) =>
            {
                byte meMsb = me.Msb;
                byte meLsb = me.Lsb;
                byte meEventType = me.EventType;
                 sendKey(meEventType, meMsb, meLsb, shift);
            };
            player.Play();
        }

        private void sendKeyWithoutShift(int meEventType, int meMsb, int meLsb)
        {
            sendKey((byte) meEventType, (byte) meMsb, (byte) meLsb, 0);
        }

        private void sendKey(byte meEventType, byte meMsb, byte meLsb, int shift)
        {
            int val;
            switch (meEventType)
            {
                case MidiEvent.Meta:
                    //case MidiEvent.Reset:
                    listBox1.Items.Add("MidiEvent.Meta");
                    break;
                case MidiEvent.ActiveSense:
                    listBox1.Items.Add("MidiEvent.ActiveSense");
                    break;
                case MidiEvent.MidiStop:
                    listBox1.Items.Add("MidiEvent.MidiStop");
                    break;
                case MidiEvent.MidiContinue:
                    listBox1.Items.Add("MidiEvent.MidiContinue");
                    break;
                case MidiEvent.MidiStart:
                    listBox1.Items.Add("MidiEvent.MidiStart");
                    break;
                case MidiEvent.MidiTick:
                    listBox1.Items.Add("MidiEvent.MidiTick");
                    break;
                case MidiEvent.MidiClock:
                    listBox1.Items.Add("MidiEvent.MidiClock");
                    break;
                case MidiEvent.EndSysEx:
                    //case MidiEvent.SysEx2:
                    listBox1.Items.Add("MidiEvent.EndSysEx");
                    break;
                case MidiEvent.TuneRequest:
                    listBox1.Items.Add("MidiEvent.TuneRequest");
                    break;
                case MidiEvent.SongPositionPointer:
                    listBox1.Items.Add("MidiEvent.SongPositionPointer");
                    break;
                case MidiEvent.MtcQuarterFrame:
                    listBox1.Items.Add("MidiEvent.MtcQuarterFrame");
                    break;
                case MidiEvent.SysEx1:
                    listBox1.Items.Add("MidiEvent.SysEx1");
                    break;
                case MidiEvent.Pitch:
                    listBox1.Items.Add("MidiEvent.Pitch");
                    break;
                case MidiEvent.CAf:
                    listBox1.Items.Add("MidiEvent.CAf");
                    break;
                case MidiEvent.Program:
                    listBox1.Items.Add("MidiEvent.Program");
                    break;
                case MidiEvent.CC:
                    listBox1.Items.Add("MidiEvent.CC");
                    break;
                case MidiEvent.PAf:
                    listBox1.Items.Add("MidiEvent.PAf");
                    break;
                case MidiEvent.SongSelect:
                    listBox1.Items.Add("MidiEvent.SongSelect");
                    break;
                case MidiEvent.NoteOn:
                    listBox1.Items.Add("MidiEvent.NoteOn");
                    val = meMsb + shift;
                    string key;
                    key = val.ToString();
                    if (mapping.ContainsKey(key))
                    {
                        int c = meLsb;
                        if (c == 0)
                        {
                            WinApiUtils.keybd_event((byte) letter[mapping[key]], 0, 2, 0);
                            textBox1.Text += ")";
                            textBox1.Text += " ";
                        }
                        else
                        {
                            //SendKeys.Send(mapping[key]);
                            WinApiUtils.keybd_event((byte) letter[mapping[key]], 0, 0, 0);
                            //WinApiUtils.PostMessage(calculatorHandle, WinApiUtils.WM_KEY_DOWN, (Keys)Enum.Parse(typeof(Keys), mapping[key].ToUpper()), letter[mapping[key]]);
                            textBox1.Text += "(" + mapping[key];
                            textBox1.Text += ";";
                        }
                    }
                    else
                    {
                        textBox1.Text += "[" + key + "]";
                    }

                    break;
                case MidiEvent.NoteOff:
                    listBox1.Items.Add("MidiEvent.NoteOff");
                    val = meMsb + shift;
                    key = val.ToString();
                    if (mapping.ContainsKey(key))
                    {
                        //SendKeys.Send(mapping[key]);
                        //WinApiUtils.PostMessage(calculatorHandle, WinApiUtils.WM_KEY_UP, (Keys)Enum.Parse(typeof(Keys), mapping[key].ToUpper()), letter[mapping[key]]);
                        WinApiUtils.keybd_event((byte) letter[mapping[key]], 0, 2, 0);
                        textBox1.Text += ")";
                        textBox1.Text += " ";
                    }

                    break;
                default:
                    //textBox1.Text += me.EventType;
                    //textBox1.Text += "\r\n";
                    break;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            player.Stop();
        }
    }
}
