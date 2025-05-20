using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelBlazeMind.App.Behaviors
{
    public class SendEventMessageBehavior : Behavior<Button>
    { 
                public SendEventMessageBehavior()
        {

        }

        public static readonly BindableProperty EventMessageProperty =
            BindableProperty.Create(
                nameof(EventMessage),
                typeof(string),
                typeof(SendEventMessageBehavior));

        public string EventMessage
        {
            get => (string)GetValue(EventMessageProperty);
            set => SetValue(EventMessageProperty, value);
        }


        public Button? AttachedObject
        {
            get; private set;
        }
        protected override void OnAttachedTo(Button bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.Clicked += Bindable_Clicked;
            AttachedObject = bindable;
        }
        protected override void OnDetachingFrom(Button bindable)
        {
          
            bindable.Clicked -= Bindable_Clicked;
            AttachedObject = null;
            base.OnDetachingFrom(bindable);    
        }

        private void Bindable_Clicked(object? sender, EventArgs e)
        {
            MVVMSidekick.EventRouting.EventRouter.Instance.RaiseEvent(AttachedObject, EventMessage, nameof(IButton.Clicked));
        }
    }
}