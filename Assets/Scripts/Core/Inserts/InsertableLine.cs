using System.Collections.Generic;

using Core.Module;

using Dominion;

using UnityEngine;

namespace Visuals.Module
{
    public class InsertableLine : MonoBehaviour
    {
        public int fontSize;

        public void Set(List<Value> arguments)
        {
            MegaUtils.ClearChildren(transform);
            foreach(var val in arguments)
            {
                InsertValue(val);
            }
        }

        public void InsertValue(Value value)
        {
            InsertableText txt = Instantiate(CDM.GetPrefab<InsertableText>("InsertableText"), transform);
            txt._text.fontSize = fontSize;
            txt.text = value.GetString();
        }
    }
}