using UnityEngine;
using System.Collections;
using SpacedOut.Utils;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Translator.instance.SetLanguage(Translator.Language.Danish);
        print(Translator.instance.Get("testing"));
        print(Translator.instance.Get("title"));
        print(Translator.instance.Get("text"));
        print(Translator.instance.Get("play"));
    }
}
