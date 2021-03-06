namespace InControl
{
	/* @cond nodoc */
	[AutoDiscover, Preserve]
	public class PlayStation4MacFirefoxUnityProfile : UnityInputDeviceProfile
	{
		public PlayStation4MacFirefoxUnityProfile()
		{
			Name = "PlayStation 4 Controller";
			Meta = "PlayStation 4 Controller on Mac Firefox";

			DeviceClass = InputDeviceClass.Controller;
			DeviceStyle = InputDeviceStyle.PlayStation4;

			IncludePlatforms = new[]
			{
				"Mac Firefox"
			};

			JoystickNames = new[]
			{
				"54c-5c4-Wireless Controller",
				"54c-9cc-Wireless Controller"
			};

			LastResortRegex = "Wireless Controller";

			ButtonMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Cross",
					Target = InputControlType.Action1,
					Source = Button( 1 ),
				},
				new InputControlMapping
				{
					Handle = "Circle",
					Target = InputControlType.Action2,
					Source = Button( 2 ),
				},
				new InputControlMapping
				{
					Handle = "Square",
					Target = InputControlType.Action3,
					Source = Button( 0 ),
				},
				new InputControlMapping
				{
					Handle = "Triangle",
					Target = InputControlType.Action4,
					Source = Button( 3 ),
				},
				new InputControlMapping
				{
					Handle = "Left Bumper",
					Target = InputControlType.LeftBumper,
					Source = Button( 4 ),
				},
				new InputControlMapping
				{
					Handle = "Right Bumper",
					Target = InputControlType.RightBumper,
					Source = Button( 5 ),
				},
				new InputControlMapping
				{
					Handle = "Share",
					Target = InputControlType.Share,
					Source = Button( 8 ),
				},
				new InputControlMapping
				{
					Handle = "Options",
					Target = InputControlType.Options,
					Source = Button( 9 ),
				},
				new InputControlMapping
				{
					Handle = "Left Stick Button",
					Target = InputControlType.LeftStickButton,
					Source = Button( 10 ),
				},
				new InputControlMapping
				{
					Handle = "Right Stick Button",
					Target = InputControlType.RightStickButton,
					Source = Button( 11 ),
				},
				new InputControlMapping
				{
					Handle = "PlayStation",
					Target = InputControlType.System,
					Source = Button( 12 ),
				},
				new InputControlMapping
				{
					Handle = "Touch Pad Button",
					Target = InputControlType.TouchPadButton,
					Source = Button( 13 ),
				},
				new InputControlMapping
				{
					Handle = "DPad Up",
					Target = InputControlType.DPadUp,
					Source = Button( 14 ),
				},
				new InputControlMapping
				{
					Handle = "DPad Down",
					Target = InputControlType.DPadDown,
					Source = Button( 15 ),
				},
				new InputControlMapping
				{
					Handle = "DPad Left",
					Target = InputControlType.DPadLeft,
					Source = Button( 16 ),
				},
				new InputControlMapping
				{
					Handle = "DPad Right",
					Target = InputControlType.DPadRight,
					Source = Button( 17 ),
				},
			};

			AnalogMappings = new[]
			{
				new InputControlMapping
				{
					Handle = "Left Stick Left",
					Target = InputControlType.LeftStickLeft,
					Source = Analog( 0 ),
					SourceRange = InputRange.ZeroToMinusOne,
					TargetRange = InputRange.ZeroToOne,
				},
				new InputControlMapping
				{
					Handle = "Left Stick Right",
					Target = InputControlType.LeftStickRight,
					Source = Analog( 0 ),
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne,
				},
				new InputControlMapping
				{
					Handle = "Left Stick Up",
					Target = InputControlType.LeftStickUp,
					Source = Analog( 1 ),
					SourceRange = InputRange.ZeroToMinusOne,
					TargetRange = InputRange.ZeroToOne,
				},
				new InputControlMapping
				{
					Handle = "Left Stick Down",
					Target = InputControlType.LeftStickDown,
					Source = Analog( 1 ),
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne,
				},
				new InputControlMapping
				{
					Handle = "Right Stick Left",
					Target = InputControlType.RightStickLeft,
					Source = Analog( 2 ),
					SourceRange = InputRange.ZeroToMinusOne,
					TargetRange = InputRange.ZeroToOne,
				},
				new InputControlMapping
				{
					Handle = "Right Stick Right",
					Target = InputControlType.RightStickRight,
					Source = Analog( 2 ),
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne,
				},
				new InputControlMapping
				{
					Handle = "Left Trigger",
					Target = InputControlType.LeftTrigger,
					Source = Analog( 3 ),
					SourceRange = InputRange.MinusOneToOne,
					TargetRange = InputRange.ZeroToOne,
					IgnoreInitialZeroValue = true
				},
				new InputControlMapping
				{
					Handle = "Right Trigger",
					Target = InputControlType.RightTrigger,
					Source = Analog( 4 ),
					SourceRange = InputRange.MinusOneToOne,
					TargetRange = InputRange.ZeroToOne,
					IgnoreInitialZeroValue = true
				},
				new InputControlMapping
				{
					Handle = "Right Stick Up",
					Target = InputControlType.RightStickUp,
					Source = Analog( 5 ),
					SourceRange = InputRange.ZeroToMinusOne,
					TargetRange = InputRange.ZeroToOne,
				},
				new InputControlMapping
				{
					Handle = "Right Stick Down",
					Target = InputControlType.RightStickDown,
					Source = Analog( 5 ),
					SourceRange = InputRange.ZeroToOne,
					TargetRange = InputRange.ZeroToOne,
				},
			};
		}
	}

	/* @endcond */
}
