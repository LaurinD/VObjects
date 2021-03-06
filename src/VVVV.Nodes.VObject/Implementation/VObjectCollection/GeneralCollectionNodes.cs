﻿using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;

using VVVV.Packs.VObjects;

namespace VVVV.Nodes.VObjects
{
    [PluginInfo(Name = "Info", Category = "VObjectCollection")]
    public class VObjectCollectionInfoNode : IPluginEvaluate
    {
        [Input("Input Collection")]
        public Pin<object> FInput;

        [Output("Name")]
        public ISpread<string> FName;
        [Output("Debug")]
        public ISpread<string> FDebug;
        [Output("Age")]
        public ISpread<double> FAge;
        [Output("Children")]
        public ISpread<ISpread<string>> FChildren;

        public void Evaluate(int SpreadMax)
        {
            if(FInput.IsConnected)
            {
                FName.SliceCount = FInput.SliceCount;
                FDebug.SliceCount = FInput.SliceCount;
                FAge.SliceCount = FInput.SliceCount;
                FChildren.SliceCount = FInput.SliceCount;
                for(int i=0; i<FInput.SliceCount; i++)
                {
                    if (FInput[i] is VObjectCollection)
                    {
                        VObjectCollection Content = FInput[i] as VObjectCollection;
                        FName[i] = Content.Name;
                        FDebug[i] = Content.Debug;
                        FAge[i] = Content.Age.Elapsed.TotalSeconds;
                        FChildren[i].SliceCount = 0;
                        foreach (string k in Content.Children.Keys)
                        {
                            FChildren[i].Add(k);
                        }
                    }
                }
            }
            else
            {
                FName.SliceCount = 0;
                FDebug.SliceCount = 0;
                FAge.SliceCount = 0;
                FChildren.SliceCount = 0;
            }
        }
    }

    [PluginInfo(Name = "ResetAge", Category = "VObjectCollection", AutoEvaluate = true)]
    public class VObjectCollectionAgeNode : IPluginEvaluate
    {
        [Input("Input Collection")]
        public Pin<object> FInput;
        [Input("Reset", IsBang = true)]
        public ISpread<bool> FReset;

        [Output("Age")]
        public ISpread<double> FAge;

        public void Evaluate(int SpreadMax)
        {
            if (FInput.IsConnected)
            {
                FAge.SliceCount = FInput.SliceCount;
                for (int i = 0; i < FInput.SliceCount; i++)
                {
                    if (FInput[i] is VObjectCollection)
                    {
                        VObjectCollection Content = FInput[i] as VObjectCollection;
                        FAge[i] = Content.Age.Elapsed.TotalSeconds;
                        if (FReset[i]) Content.Age.Restart();
                    }
                }
            }
            else
            {
                FAge.SliceCount = 0;
            }
        }
    }

    [PluginInfo(Name = "Debug", Category = "VObjectCollection", AutoEvaluate = true)]
    public class VObjectCollectionDebugNode : IPluginEvaluate
    {
        [Input("Input Collection")]
        public Pin<object> FInput;
        [Input("Debug")]
        public ISpread<string> FDebug;
        [Input("Set", IsBang = true)]
        public ISpread<bool> FSet;

        [Output("Debug Out")]
        public ISpread<string> FDebugOut;

        public void Evaluate(int SpreadMax)
        {
            if (FInput.IsConnected)
            {
                FDebugOut.SliceCount = FInput.SliceCount;
                for (int i = 0; i < FInput.SliceCount; i++)
                {
                    if (FInput[i] is VObjectCollection)
                    {
                        VObjectCollection Content = FInput[i] as VObjectCollection;
                        if (FSet[i]) Content.Debug = FDebug[i];
                        FDebugOut[i] = Content.Debug;
                    }
                }
            }
            else
            {
                FDebugOut.SliceCount = 0;
            }
        }
    }
}
