using Commons.Music.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenshinLyreAutoPlay
{
    class TuneUtils
    {

        private static Dictionary<string, string> mapping = new Dictionary<string, string>
         {
            {"48", "z"},{ "50", "x"},{ "52", "c"},{ "53", "v"},{ "55", "b"},{ "57", "n"},{ "59", "m"},{ "60", "a"},{ "62", "s"},{ "64", "d"},{ "65", "f"},{ "67", "g"},{ "69", "h"},{ "71", "j"},{ "72", "q"},{ "74", "w"},{ "76", "e"},{ "77", "r"},{ "79", "t"},{ "81", "y"},{ "83", "u"}
         };


        public static int getBestShift(MidiMusic midiMusic) {
            int shift = 0;
            int oldHit = 0;
            for (int i = -21; i < 22; i++) {
                int hit = 0;
                foreach (MidiTrack track in midiMusic.Tracks) {
                    foreach (MidiMessage mm in track.Messages) {
                        int tempo = mm.Event.Msb + i;
                        if (mapping.ContainsKey(tempo.ToString())) {
                            hit++;
                        }
                    }
                }
                if (hit > oldHit) {
                    shift = i;
                    oldHit = hit;
                }
            }
            return shift;
        }
    }
}
