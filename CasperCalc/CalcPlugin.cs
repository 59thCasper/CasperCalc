using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace CasperCalc
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class mainPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.casper.CasperCalc";
        private const string PluginName = "CasperCalc";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);
        private string input = "";
        private string result = "";
        private bool showCalculator = false;
        private bool inputFieldFocused = false;
        private float centeredX;
        private float centeredY;
        private Rect calculatorWindowRect;

        private void Awake()
        {
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();

            Log.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }

        void Start()
        {
            centeredX = (Screen.width - 300) / 2;
            centeredY = ((Screen.height - 200) / 2) + 300;
            calculatorWindowRect = new Rect(centeredX, centeredY, 400, 200);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                showCalculator = !showCalculator;
                if (showCalculator)
                {
                    inputFieldFocused = true;
                }
            }
        }

        private void OnGUI()
        {
            if (showCalculator)
            {
                GUI.color = Color.white;
                calculatorWindowRect = GUILayout.Window(0, calculatorWindowRect, CalculatorWindow, "CasperCalc");
            }
        }

        private void CalculatorWindow(int windowID)
        {
            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = 24
            };

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24
            };

            if (GUI.Button(new Rect(calculatorWindowRect.width - 35, 5, 30, 30), "X"))
            {
                showCalculator = false;
            }
            GUILayout.Space(40);
            GUILayout.BeginVertical();

            GUI.SetNextControlName("InputField");
            string newInput = GUILayout.TextField(input, textFieldStyle, GUILayout.Height(50));
            newInput = ValidateInput(newInput);
            if (newInput != input)
            {
                input = newInput;
                CalculateResult();
            }

            if (Event.current.type == EventType.Repaint && inputFieldFocused)
            {
                GUI.FocusControl("InputField");
                inputFieldFocused = false;
            }

            GUILayout.Label("Result: " + result, labelStyle, GUILayout.Height(50));
            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, calculatorWindowRect.width, 20));
        }

        private void CalculateResult()
        {
            try
            {
                result = EvaluateExpression(input).ToString();
            }
            catch
            {
                result = "Error";
            }
        }

        private double EvaluateExpression(string expression)
        {
            var dataTable = new System.Data.DataTable();
            return double.Parse(dataTable.Compute(expression, "").ToString());
        }

        private string ValidateInput(string input)
        {
            string validCharacters = "0123456789+-*/().";
            string validatedInput = "";
            foreach (char c in input)
            {
                if (validCharacters.Contains(c.ToString()))
                {
                    validatedInput += c;
                }
            }
            return validatedInput;
        }
    }
}



