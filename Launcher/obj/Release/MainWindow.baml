   M S B A M L   `   `   `  ���� C  ?Launcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:LauncherLauncher      Launcher.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     (localclr-namespace:Launcher     -     � WindowStartupLocation$  CenterScreen=� � Title$ Launcher��$	��450��$	��800��.��� .+��� $
��Left=�$	��409��$	��Top=�$	��792��$��background.jpg��$��UniformToFill=��� -    ��Name$ VersionText��$
��Left=�$��
10,382,0,0q�$
��Wrap=�$�� ��$	��Top=�$��27��$	��205��
��������� -    ��Name$ 
PlayButton��$��Checking for updates��$
��Left=�$��226,336,0,0q�$	��Top=�$	��315��$��51�����	#FF8D2BB2
�������$��16(�   PlayButtonStyle#���    � 	Resources   FocusVisual �� Property �� Value J� SnapsToDevicePixels �� StrokeDashArray  Button.Static.Background	 �� Color  Button.Static.Border  Button.MouseOver.Background  Button.MouseOver.Border  Button.Pressed.Background  Button.Pressed.Border  Button.Disabled.Background 	 Button.Disabled.Border 
 Button.Disabled.Foreground
 �� 
TargetType �� 
TargetType ��Name ��Name �� Triggers P� Property P� Value �� 
TargetName��%x  &	       &	 N     &	 c     &	 x     &	 �     &	 �     &	 �     &	 �     &		 �     &	
 �     &	      7  7  7  7  7  7  7  7  7 	 7 
 �� .���  � �� �� .R��� $��2q� . #��� ��$��1��$	 1 2R��� $	 	#FFDDDDDD���� $	 	#FF707070���� $	 	#FF8D2BB2���� $	 	#FF3C7FB1���� $	 	#FF8D2BB2���� $	 	#FF2C628B���� $	 	#FFF4F4F4���� $	 	#FFADB2B5���� $	 	#FF838383���� "
 ��.���  �@�FocusVisualStyle8   ��  � ��8  ��  � ��8  ��  � ��# � ����  � ��$ 1q���  � ��$ Center=���  � ��$ Center=���  � ��$ 1q���  � �� �� " ��.R��� $ border��#��z��#��z��#��z�� . ._��� $ contentPresenter��|�.  #��z��#��z����. # z #��z�� P�  �@��IsDefaulted . . ��� $ border��# � �� � ��P�  �@J�IsMouseOver . . ��� $ border��8   � ���� $ border��8   � ��P�  � �� . . ��� $ border��8   � ���� $ border��8   � ��P�  � {� .  . ��� $ border��8   � ���� $ border��8   � ���� $ contentPresenter��8 	  � ���� ���	#FFF4F4F5$
��Left=�$��58��$��499,232,0,0q�
���   �$	��Top=�$	��264���� -   $ 
txtRootDir��$
��Left=�$��505,267,0,0q�$
��Wrap=�$	��Top=�$	��251���� -   $ 
lblRootDir��$
��Left=�$��505,239,0,0q�$
��Wrap=�$��Game Install Directory:��$	��Top=�$	��251���� -   $ btnChooseRootDir��$��Choose Game Install Dir��$
��Left=�$��553,268,0,0q�$	��Top=�$	��160��