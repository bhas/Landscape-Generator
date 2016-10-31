using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace SpacedOut.Utils
{
    public class Translator : Singleton<Translator>
    {
        public enum Language
        {
            Danish, English
        }
        private Language language;
        private Dictionary<string, string> danish;
        private Dictionary<string, string> english;
        
        protected override void Awake()
        {
            base.Awake();
            language = Language.English;
            LoadTranslations();
        }

        public string Get(string key)
        {
            if (language == Language.Danish)
            {
                if (danish.ContainsKey(key))
                    return danish[key];
            }
            else
            {
                if (english.ContainsKey(key))
                    return english[key];
            }
            Debug.LogWarning("Translation key not found: " + key);
            return key;
        }

        public void SetLanguage(Language lan)
        {
            language = lan;
        }

        private void LoadTranslations()
        {
            danish = new Dictionary<string, string>();
            english = new Dictionary<string, string>();
            TextAsset asset = Resources.Load<TextAsset>("Translations");
            var lines = asset.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            for (int i = 1; i <  lines.Length; i++)
            {
                var vals = lines[i].Split('\t');
                english[vals[0]] = vals[1];
                danish[vals[0]] = vals[2];
            }
        } 
    }
}

