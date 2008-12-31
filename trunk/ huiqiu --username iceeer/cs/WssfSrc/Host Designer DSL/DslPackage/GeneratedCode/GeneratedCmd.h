﻿
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//
// This file contains generated default command definitions.
// Additional command definitions should be added to CustomCmd.ctc.
//

///////////////////////////////////////////////////////////////////////////////
// CTC command IDs - MUST be kept in sync with Constants.cs

#define menuidContext		0x10000
#define grpidContextMain	0x20000
#define menuidExplorer		0x10001
#define cmdidViewExplorer	0x0001

///////////////////////////////////////////////////////////////////////////////
// CTC macros

#define OI_NOID		guidOfficeIcon:msotcidNoIcon
#define DIS_DEF		DEFAULTDISABLED | DEFAULTINVISIBLE | DYNAMICVISIBILITY
#define VIS_DEF		COMMANDWELLONLY	

///////////////////////////////////////////////////////////////////////////////
// Menu definitions

#define GENERATED_MENUS \
	guidCmdSet:menuidContext, guidCmdSet:menuidContext,	0x0000,	CONTEXT|ALWAYSCREATE, "HostDesigner Designer Context Menu", "HostDesigner Context"; \
	guidCmdSet:menuidExplorer, guidCmdSet:menuidExplorer, 0x0000, CONTEXT|ALWAYSCREATE, "HostDesigner Explorer Context Menu", "Host Explorer"; \

///////////////////////////////////////////////////////////////////////////////
// Group definitions

#define GENERATED_GROUPS \
	guidCmdSet:grpidContextMain, guidCmdSet:grpidContextMain,	0x0010; \


///////////////////////////////////////////////////////////////////////////////
// Command definitions

#define GENERATED_BUTTONS \
	guidCmdSet:cmdidViewExplorer, guidSHLMainMenu:IDG_VS_WNDO_OTRWNDWS1, 0x0100,	OI_NOID, BUTTON, DIS_DEF, "Host Explorer";


///////////////////////////////////////////////////////////////////////////////
// Command placement definitions

#define GENERATED_CMDPLACEMENT \
	guidVSStd97:cmdidDelete, guidCmdSet:grpidContextMain, 0x0010; \
	guidSHLMainMenu:IDG_VS_CTXT_SOLUTION_PROPERTIES, guidCmdSet:menuidContext, 0x0500; \
	guidCommonModelingMenu:grpidExplorerMenuGroup, guidCmdSet:menuidExplorer, 0x0010; \
	guidSHLMainMenu:IDG_VS_CTXT_SOLUTION_PROPERTIES, guidCmdSet:menuidExplorer, 0x0020; \
	guidCommonModelingMenu:grpidValidateCommands, guidCmdSet:menuidExplorer, 0x0030; \
    

///////////////////////////////////////////////////////////////////////////////
// Command visibility definitions

#define GENERATED_VISIBILITY \
	guidCmdSet:cmdidViewExplorer, guidEditor; \

///////////////////////////////////////////////////////////////////////////////
// CTC guids - MUST be kept in sync with GeneratedCmd.cs

#define guidPkg			{ 0x433e3a20, 0x4740, 0x4938, { 0x8c, 0x81, 0xad, 0xf2, 0x8d, 0x43, 0x01, 0x88 } }
#define guidEditor		{ 0xdfb1660d, 0x4c34, 0x42fd, { 0x9d, 0x56, 0x52, 0xe6, 0xc3, 0x3c, 0xec, 0x4c } }


#define guidCmdSet		{ 0x7f1c6863, 0x602e, 0x41c8, { 0x85, 0x09, 0xa8, 0x64, 0x7d, 0xd2, 0x5d, 0x3b } }