﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp1.Client.Pages
{
    public partial class Counter
    {
        private int currentCount = 0;

        private void IncrementCount()
        {
            currentCount++;
        }

        string texxt = "";
        void ButtonClicked()
        {
            texxt = new Random().Next(100, 500).ToString();
        }
    }
}
