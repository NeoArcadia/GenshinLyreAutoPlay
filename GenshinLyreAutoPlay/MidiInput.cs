using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using NAudio.Midi;

namespace GenshinLyreAutoPlay
{
    ///<summary> 
    /// description
    ///</summary>
    /// <param name="params">paraminfo</param>
    public class MidiInput
    {
        MidiIn midiIn;

        public delegate void MidiInputEvent(int keyEvent, int note, int velocity);

        public event MidiInputEvent midiInputEvent;

    public void Start(ComboBox comboBox)
    {

        List<String> midiDevList = new List<String>();
        for (int i = 0; i < MidiIn.NumberOfDevices; i++){
            midiDevList.Add(MidiIn.DeviceInfo(i).ProductName);
        }
        if (midiDevList.Count == 0){
            midiDevList.Add("none");
        }

        comboBox.Items.AddRange(midiDevList.ToArray());
        comboBox.SelectedIndexChanged+=(sender, args) =>
        {
            Console.WriteLine("发送者:{0},事件内容:{1}", sender.ToString(), args.ToString());
            Console.WriteLine("选取item id:{0}",  comboBox.SelectedIndex);
            if (midiIn!=null)
            {
                midiIn.Close();
            }
            if (comboBox.SelectedIndex < MidiIn.NumberOfDevices){
                midiIn = new MidiIn(comboBox.SelectedIndex);
                midiIn.MessageReceived += midiIn_MsgReceiver;
                midiIn.ErrorReceived += midiIn_ErrorReceiver;
                midiIn.Start();
            }
        };
    }

    private void midiIn_MsgReceiver(object sender, MidiInMessageEventArgs e) {
        Console.WriteLine("msg Time {0} Msg 0x{1:X8} Event {2}", e.Timestamp, e.RawMessage, e.MidiEvent);
        Console.WriteLine("msg MidiEvent Cmd Code:{0};Channel:{1}", e.MidiEvent.CommandCode, e.MidiEvent.Channel);
        if (MidiEvent.IsNoteOn(e.MidiEvent))
        {
            midiInputEvent(e.RawMessage & 0xFF, e.RawMessage >> 8 & 0xFF, e.RawMessage >> 16 & 0xFF);
        }

        if (MidiEvent.IsNoteOff(e.MidiEvent)){
            midiInputEvent(e.RawMessage & 0xFF, e.RawMessage >> 8 & 0xFF, e.RawMessage >> 16 & 0xFF);
        }
    }
    
    
    private void midiIn_ErrorReceiver(object sender, MidiInMessageEventArgs e) {
        Console.WriteLine("err Time {0} Msg 0x{1:X8} Event {2}", e.Timestamp, e.RawMessage, e.MidiEvent);
    }
    }
}