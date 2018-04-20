using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace iControls
{
    public partial class iComponent : Component
    {
        public iComponent()
        {
            InitializeComponent();
        }

        public iComponent(IContainer a_container)
        {
            a_container.Add(this);

            InitializeComponent();
        }
    }
}
