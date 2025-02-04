﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BMW.Rheingold.Psdz.Client;
using log4net;
using PsdzClient;
using PsdzClient.Programming;
using WebPsdzClient.App_Data;

namespace WebPsdzClient
{
    public partial class _Default : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(_Default));

        protected override void InitializeCulture()
        {
            SessionContainer.SetLogInfo(Session.SessionID);
            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            string language = sessionContainer.GetLanguage();
            if (!string.IsNullOrEmpty(language))
            {
                try
                {
                    CultureInfo culture = CultureInfo.CreateSpecificCulture(language.ToLowerInvariant());
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                    Culture = culture.TwoLetterISOLanguageName;
                    UICulture = culture.TwoLetterISOLanguageName;
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("InitializeCulture Exception: {0}", ex.Message);
                }
            }
            base.InitializeCulture();
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            //log.InfoFormat("_Default Page_Init");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //log.InfoFormat("_Default Page_Load");
            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            if (!IsPostBack)
            {
                UpdateStatus();
                UpdateCurrentOptions();
                UpdateTimerPanel();
            }
            else
            {
                Control postbackControl = GetPostBackControl(this);
                if (postbackControl == UpdatePanelStatus)
                {
                    UpdateStatus(true);
                }

                if (sessionContainer.RefreshOptions)
                {
                    sessionContainer.RefreshOptions = false;
                    UpdateOptions();
                }
            }
        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            //log.InfoFormat("_Default Page_Unload");
        }

        protected void ButtonStopHost_Click(object sender, EventArgs e)
        {
            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            if (sessionContainer.TaskActive)
            {
                return;
            }

            sessionContainer.StopProgrammingService(Global.IstaFolder);
        }

        protected void ButtonConnect_OnClick(object sender, EventArgs e)
        {
            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            if (sessionContainer.TaskActive)
            {
                return;
            }

            sessionContainer.ConnectVehicle(Global.IstaFolder);
        }

        protected void ButtonDisconnect_OnClick(object sender, EventArgs e)
        {
            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            if (sessionContainer.TaskActive)
            {
                return;
            }

            sessionContainer.DisconnectVehicle();
        }

        protected void ButtonCreateOptions_OnClick(object sender, EventArgs e)
        {
            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            if (sessionContainer.TaskActive)
            {
                return;
            }

            sessionContainer.VehicleFunctions(ProgrammingJobs.OperationType.CreateOptions);
        }

        protected void ButtonModifyFa_OnClick(object sender, EventArgs e)
        {
            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            if (sessionContainer.TaskActive)
            {
                return;
            }

            sessionContainer.ProgrammingJobs.UpdateTargetFa();
            sessionContainer.VehicleFunctions(ProgrammingJobs.OperationType.BuildTalModFa);
        }

        protected void ButtonExecuteTal_OnClick(object sender, EventArgs e)
        {
            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            if (sessionContainer.TaskActive)
            {
                return;
            }

            if (sessionContainer.TaskActive)
            {
                return;
            }

            sessionContainer.VehicleFunctions(ProgrammingJobs.OperationType.ExecuteTal);
        }

        protected void ButtonAbort_OnClick(object sender, EventArgs e)
        {
            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            if (!sessionContainer.TaskActive)
            {
                return;
            }

            sessionContainer.Cancel();
        }

        protected void ButtonMsgOk_OnClick(object sender, EventArgs e)
        {
            log.InfoFormat("_Default ButtonMsgOk_OnClick");

            ModalPopupExtenderMsg.Hide();

            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            sessionContainer.ShowMessageModal = null;
            sessionContainer.ShowMessageModalResult = true;
            sessionContainer.MessageWaitEvent.Set();
        }

        protected void ButtonMsgYes_OnClick(object sender, EventArgs e)
        {
            log.InfoFormat("_Default ButtonMsgYes_OnClick");

            ModalPopupExtenderMsg.Hide();

            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            sessionContainer.ShowMessageModal = null;
            sessionContainer.ShowMessageModalResult = true;
            sessionContainer.MessageWaitEvent.Set();
        }

        protected void ButtonMsgNo_OnClick(object sender, EventArgs e)
        {
            log.InfoFormat("_Default ButtonMsgNo_OnClick");

            ModalPopupExtenderMsg.Hide();

            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            sessionContainer.ShowMessageModal = null;
            sessionContainer.ShowMessageModalResult = false;
            sessionContainer.MessageWaitEvent.Set();
        }

        protected void DropDownListOptionType_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            if (sessionContainer.TaskActive)
            {
                return;
            }

            PdszDatabase.SwiRegisterEnum? selectedSwiRegister = null;
            ListItem listItemSelect = DropDownListOptionType.SelectedItem;
            if (listItemSelect != null)
            {
                if (Enum.TryParse(listItemSelect.Value, true, out PdszDatabase.SwiRegisterEnum swiRegister))
                {
                    selectedSwiRegister = swiRegister;
                }
            }

            if (sessionContainer.SelectedSwiRegister != selectedSwiRegister)
            {
                sessionContainer.SelectedSwiRegister = selectedSwiRegister;
                UpdateOptions();
            }
        }

        protected void CheckBoxListOptions_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            try
            {
                if (sessionContainer.TaskActive)
                {
                    return;
                }

                Request.ValidateInput();
                string eventArgs = Request.Form["__EVENTTARGET"];
                if (string.IsNullOrEmpty(eventArgs))
                {
                    return;
                }
                string[] checkedBox = eventArgs.Split('$');
                if (checkedBox.Length < 1)
                {
                    return;
                }

                if (!int.TryParse(checkedBox[checkedBox.Length - 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int selectedIndex))
                {
                    log.ErrorFormat("CheckBoxListOptions_OnSelectedIndexChanged Invalid checkbox: {0}", checkedBox[checkedBox.Length - 1]);
                    return;
                }

                if (selectedIndex < 0 || selectedIndex >= CheckBoxListOptions.Items.Count)
                {
                    log.ErrorFormat("CheckBoxListOptions_OnSelectedIndexChanged Invalid index: {0}", selectedIndex);
                    return;
                }

                ListItem listItem = CheckBoxListOptions.Items[selectedIndex];
                if (!listItem.Enabled)
                {
                    log.ErrorFormat("CheckBoxListOptions_OnSelectedIndexChanged Disabled: {0}", listItem.Text);
                    return;
                }

                string optionId = listItem.Value;
                if (string.IsNullOrEmpty(optionId))
                {
                    log.ErrorFormat("CheckBoxListOptions_OnSelectedIndexChanged No ID for: {0}", listItem.Text);
                    return;
                }

                log.InfoFormat("CheckBoxListOptions_OnSelectedIndexChanged Selected: {0}", listItem.Text);
                ProgrammingJobs programmingJobs = sessionContainer.ProgrammingJobs;
                bool modified = false;
                Dictionary<PdszDatabase.SwiRegisterEnum, List<ProgrammingJobs.OptionsItem>> optionsDict = programmingJobs.OptionsDict;
                if (optionsDict != null && sessionContainer.SelectedSwiRegister.HasValue)
                {
                    if (optionsDict.TryGetValue(sessionContainer.SelectedSwiRegister.Value, out List<ProgrammingJobs.OptionsItem> optionsItems))
                    {
                        foreach (ProgrammingJobs.OptionsItem optionsItem in optionsItems)
                        {
                            if (string.Compare(optionsItem.Id, optionId, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                PdszDatabase.SwiRegisterEnum swiRegisterEnum = optionsItem.SwiRegisterEnum;
                                if (programmingJobs.SelectedOptions.Count > 0)
                                {
                                    PdszDatabase.SwiRegisterEnum swiRegisterEnumCurrent = programmingJobs.SelectedOptions[0].SwiRegisterEnum;
                                    if (PdszDatabase.GetSwiRegisterGroup(swiRegisterEnum) != PdszDatabase.GetSwiRegisterGroup(swiRegisterEnumCurrent))
                                    {
                                        programmingJobs.SelectedOptions.Clear();
                                    }
                                }

                                if (programmingJobs.SelectedOptions != null)
                                {
                                    List<ProgrammingJobs.OptionsItem> combinedOptionsItems = programmingJobs.GetCombinedOptionsItems(optionsItem, optionsItems);
                                    if (listItem.Selected)
                                    {
                                        if (!programmingJobs.SelectedOptions.Contains(optionsItem))
                                        {
                                            programmingJobs.SelectedOptions.Add(optionsItem);
                                        }

                                        if (combinedOptionsItems != null)
                                        {
                                            foreach (ProgrammingJobs.OptionsItem combinedItem in combinedOptionsItems)
                                            {
                                                if (!programmingJobs.SelectedOptions.Contains(combinedItem))
                                                {
                                                    programmingJobs.SelectedOptions.Add(combinedItem);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        programmingJobs.SelectedOptions.Remove(optionsItem);

                                        if (combinedOptionsItems != null)
                                        {
                                            foreach (ProgrammingJobs.OptionsItem combinedItem in combinedOptionsItems)
                                            {
                                                programmingJobs.SelectedOptions.Remove(combinedItem);
                                            }
                                        }
                                    }
                                }

                                modified = true;
                                break;
                            }
                        }
                    }
                }

                if (modified)
                {
                    log.InfoFormat("CheckBoxListOptions_OnSelectedIndexChanged Modified, Updating FA");
                    PsdzClient.Programming.PsdzContext psdzContext = sessionContainer.ProgrammingJobs.PsdzContext;
                    if (psdzContext?.Connection != null)
                    {
                        psdzContext.Tal = null;
                    }

                    sessionContainer.ProgrammingJobs.UpdateTargetFa();
                    UpdateOptions();
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("CheckBoxListOptions_OnSelectedIndexChanged Exception: {0}", ex.Message);
            }
        }

        protected void TimerUpdate_Tick(object sender, EventArgs e)
        {
            //log.InfoFormat("_Default TimerUpdate_Tick");

            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            sessionContainer.UpdateDisplay(false);
            UpdateTimerPanel();
        }

        private SessionContainer GetSessionContainer()
        {
            if (Session.Contents[Global.SessionContainerName] is SessionContainer sessionContainer)
            {
                return sessionContainer;
            }

            log.ErrorFormat("GetSessionContainer No SessionContainer");
            return null;
        }

        private void UpdateStatus(bool updatePanel = false)
        {
            log.InfoFormat("UpdateStatus Update panel: {0}", updatePanel);

            try
            {
                SessionContainer sessionContainer = GetSessionContainer();
                if (sessionContainer == null)
                {
                    return;
                }

                bool active = sessionContainer.TaskActive;
                bool abortPossible = sessionContainer.Cts != null;
                bool hostRunning = false;
                bool vehicleConnected = false;
                bool talPresent = false;
                if (!active)
                {
                    hostRunning = PsdzServiceStarter.IsServerInstanceRunning();
                }

                ProgrammingJobs programmingJobs = sessionContainer.ProgrammingJobs;
                PsdzClient.Programming.PsdzContext psdzContext = programmingJobs.PsdzContext;
                if (psdzContext?.Connection != null)
                {
                    vehicleConnected = true;
                    talPresent = psdzContext.Tal != null;
                }

                Dictionary<PdszDatabase.SwiRegisterEnum, List<ProgrammingJobs.OptionsItem>> optionsDict = programmingJobs.OptionsDict;
                bool modifyTal = !active && hostRunning && vehicleConnected && optionsDict != null;
                ButtonStopHost.Enabled = !active && hostRunning;
                ButtonStopHost.Visible = sessionContainer.DeepObdVersion <= 0;
                ButtonConnect.Enabled = !active && !vehicleConnected;
                ButtonDisconnect.Enabled = !active && hostRunning && vehicleConnected;
                ButtonCreateOptions.Enabled = !active && hostRunning && vehicleConnected && optionsDict == null;
                ButtonModifyFa.Enabled = modifyTal;
                ButtonExecuteTal.Enabled = modifyTal && talPresent;
                ButtonAbort.Enabled = active && abortPossible;
                DropDownListOptionType.Enabled = !active && hostRunning && vehicleConnected;
                CheckBoxListOptions.Enabled = !active && hostRunning && vehicleConnected;

                TextBoxStatus.Text = sessionContainer.StatusText;
                TextBoxProgress.Text = sessionContainer.ProgressText;

                string messageText = sessionContainer.ShowMessageModal;
                if (!string.IsNullOrEmpty(messageText))
                {
                    string modalCount = sessionContainer.ShowMessageModalCount.ToString(CultureInfo.InvariantCulture);
                    if (string.Compare(HiddenFieldMsgModal.Value, modalCount, StringComparison.Ordinal) != 0)
                    {
                        bool okBtn = sessionContainer.ShowMessageModalOkBtn;
                        bool messageWait = sessionContainer.ShowMessageModalWait;
                        messageText = messageText.Replace("\r\n", "<br/>");

                        log.InfoFormat("_Default Page_Load UpdateStatus Count={0}, OKButton={1}, Wait={2}, Message='{3}'", modalCount, okBtn, messageWait, messageText);

                        LiteralMsgModal.Text = messageText;
                        ButtonMsgOk.Visible = okBtn;
                        ButtonMsgYes.Visible = !okBtn;
                        ButtonMsgNo.Visible = !okBtn;
                        ModalPopupExtenderMsg.Show();
                        HiddenFieldMsgModal.Value = modalCount;
                    }
                }

                if (updatePanel)
                {
                    UpdatePanels();
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("UpdateStatus Exception: {0}", ex.Message);
            }
        }

        private void UpdatePanels()
        {
            try
            {
                if (!UpdatePanelStatus.IsInPartialRendering)
                {
                    UpdatePanelStatus.Update();
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("UpdatePanels Exception: {0}", ex.Message);
            }

            UpdateTimerPanel();
        }

        private void UpdateTimerPanel()
        {
            try
            {
                SessionContainer sessionContainer = GetSessionContainer();
                if (sessionContainer == null)
                {
                    return;
                }

                DateTime localTime = DateTime.Now;
                DateTime utcTime = localTime.ToUniversalTime();
                string localString = localTime.ToString("HH:mm:ss");
                string utcString = utcTime.ToString("HH:mm:ss");

                string timeFormat = GetGlobalResourceObject("Global", "TimeDisplay") as string ?? string.Empty;
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format(CultureInfo.InvariantCulture, timeFormat, localString, utcString));

                int? connectTimeouts = sessionContainer.ConnectTimeouts;
                if (connectTimeouts != null && connectTimeouts.Value > 0)
                {
                    sb.Append("<br/>");
                    string connectFailFormat = GetGlobalResourceObject("Global", "InternetTimeouts") as string ?? string.Empty;
                    sb.Append(string.Format(CultureInfo.InvariantCulture, connectFailFormat, connectTimeouts.Value));
                }

                LabelLastUpdate.Text = sb.ToString();
                if (!UpdatePanelTimer.IsInPartialRendering)
                {
                    UpdatePanelTimer.Update();
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("UpdateTimerPanel Exception: {0}", ex.Message);
            }
        }

        private void UpdateOptions()
        {
            SessionContainer sessionContainer = GetSessionContainer();
            if (sessionContainer == null)
            {
                return;
            }

            if (sessionContainer.DeepObdVersion > 0)
            {
                sessionContainer.ReloadPage();
            }
            else
            {
                try
                {
                    Request.ValidateInput();
                    string url = Request.RawUrl;
                    log.InfoFormat("UpdateOptions Reload Url: {0}", url);
                    if (!string.IsNullOrEmpty(url))
                    {
                        Response.Redirect(url, false);
                    }
                }
                catch (Exception ex)
                {
                    log.ErrorFormat("UpdateOptions Exception: {0}", ex.Message);
                }
            }
        }

        private void UpdateCurrentOptions(bool updatePanel = false)
        {
            try
            {
                SessionContainer sessionContainer = GetSessionContainer();
                if (sessionContainer == null)
                {
                    return;
                }

                DropDownListOptionType.Items.Clear();

                Dictionary<PdszDatabase.SwiRegisterEnum, List<ProgrammingJobs.OptionsItem>> optionsDict = sessionContainer.ProgrammingJobs.OptionsDict;
                if (optionsDict != null)
                {
                    ProgrammingJobs programmingJobs = sessionContainer.ProgrammingJobs;
                    if (sessionContainer.SelectedSwiRegister == null)
                    {
                        sessionContainer.SelectedSwiRegister = programmingJobs.OptionTypes[0].SwiRegisterEnum;
                    }
                    foreach (ProgrammingJobs.OptionType optionTypeUpdate in programmingJobs.OptionTypes)
                    {
                        PdszDatabase.SwiRegisterGroup swiRegisterGroup = PdszDatabase.GetSwiRegisterGroup(optionTypeUpdate.SwiRegisterEnum);
                        if (swiRegisterGroup != PdszDatabase.SwiRegisterGroup.Modification)
                        {
                            if (!sessionContainer.HasDisplayOption("Hardware"))
                            {
                                continue;
                            }
                        }

                        ListItem listItem = new ListItem(optionTypeUpdate.ToString(), optionTypeUpdate.SwiRegisterEnum.ToString());
                        if (sessionContainer.SelectedSwiRegister == optionTypeUpdate.SwiRegisterEnum)
                        {
                            listItem.Selected = true;
                        }
                        DropDownListOptionType.Items.Add(listItem);
                    }
                }
                else
                {
                    sessionContainer.SelectedSwiRegister = null;
                }

                SelectOptions(sessionContainer.SelectedSwiRegister);
                PanelOptions.Visible = optionsDict != null;

                if (updatePanel)
                {
                    UpdatePanels();
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SelectOptions Exception: {0}", ex.Message);
            }
        }

        private void SelectOptions(PdszDatabase.SwiRegisterEnum? swiRegisterEnum)
        {
            try
            {
                SessionContainer sessionContainer = GetSessionContainer();
                if (sessionContainer == null)
                {
                    return;
                }

                CheckBoxListOptions.Items.Clear();

                ProgrammingJobs programmingJobs = sessionContainer.ProgrammingJobs;
                if (programmingJobs.ProgrammingService == null || programmingJobs.PsdzContext?.Connection == null)
                {
                    return;
                }

                bool replacement = false;
                if (swiRegisterEnum.HasValue)
                {
                    switch (PdszDatabase.GetSwiRegisterGroup(swiRegisterEnum.Value))
                    {
                        case PdszDatabase.SwiRegisterGroup.HwDeinstall:
                        case PdszDatabase.SwiRegisterGroup.HwInstall:
                            replacement = true;
                            break;
                    }
                }

                Dictionary<PdszDatabase.SwiRegisterEnum, List<ProgrammingJobs.OptionsItem>> optionsDict = programmingJobs.OptionsDict;
                List<PdszDatabase.SwiAction> selectedSwiActions = GetSelectedSwiActions(programmingJobs);
                List<PdszDatabase.SwiAction> linkedSwiActions = programmingJobs.ProgrammingService.PdszDatabase.ReadLinkedSwiActions(selectedSwiActions, programmingJobs.PsdzContext?.VecInfo, null);

                if (optionsDict != null && programmingJobs.SelectedOptions != null && swiRegisterEnum.HasValue)
                {
                    if (optionsDict.TryGetValue(swiRegisterEnum.Value, out List<ProgrammingJobs.OptionsItem> optionsItems))
                    {
                        foreach (ProgrammingJobs.OptionsItem optionsItem in optionsItems.OrderBy(x => x.ToString()))
                        {
                            bool itemSelected = false;
                            bool itemEnabled = true;
                            bool addItem = true;
                            int selectIndex = programmingJobs.SelectedOptions.IndexOf(optionsItem);
                            if (selectIndex >= 0)
                            {
                                if (replacement)
                                {
                                    itemSelected = true;
                                }
                                else
                                {
                                    if (selectIndex == programmingJobs.SelectedOptions.Count - 1)
                                    {
                                        itemSelected = true;
                                    }
                                    else
                                    {
                                        itemSelected = true;
                                        itemEnabled = false;
                                    }
                                }
                            }
                            else
                            {
                                if (replacement)
                                {
                                    if (optionsItem.EcuInfo == null)
                                    {
                                        addItem = false;
                                    }
                                }
                                else
                                {
                                    if (linkedSwiActions != null &&
                                        linkedSwiActions.Any(x => string.Compare(x.Id, optionsItem.SwiAction.Id, StringComparison.OrdinalIgnoreCase) == 0))
                                    {
                                        addItem = false;
                                    }
                                    else
                                    {
                                        if (!programmingJobs.ProgrammingService.PdszDatabase.EvaluateXepRulesById(optionsItem.SwiAction.Id, programmingJobs.PsdzContext?.VecInfo, null))
                                        {
                                            addItem = false;
                                        }
                                    }
                                }
                            }

                            if (addItem)
                            {
                                if (!programmingJobs.IsOptionsItemEnabled(optionsItem))
                                {
                                    itemEnabled = false;
                                }

                                ListItem listItem = new ListItem(optionsItem.ToString(), optionsItem.Id);
                                listItem.Selected = itemSelected;
                                listItem.Enabled = itemEnabled;
                                CheckBoxListOptions.Items.Add(listItem);

                                log.InfoFormat("SelectOptions Added: Text={0}, Selected={1}, Enabled={2}", listItem.Text, listItem.Selected, listItem.Enabled);
                            }
                        }
                    }
                }

                UpdatePanels();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SelectOptions Exception: {0}", ex.Message);
            }
        }

        private List<PdszDatabase.SwiAction> GetSelectedSwiActions(ProgrammingJobs programmingJobs)
        {
            if (programmingJobs.PsdzContext?.Connection == null || programmingJobs.SelectedOptions == null)
            {
                return null;
            }

            List<PdszDatabase.SwiAction> selectedSwiActions = new List<PdszDatabase.SwiAction>();
            foreach (ProgrammingJobs.OptionsItem optionsItem in programmingJobs.SelectedOptions)
            {
                if (optionsItem.SwiAction != null)
                {
                    log.InfoFormat("GetSelectedSwiActions Selected: {0}", optionsItem.SwiAction);
                    selectedSwiActions.Add(optionsItem.SwiAction);
                }
            }

            log.InfoFormat("GetSelectedSwiActions Count: {0}", selectedSwiActions.Count);

            return selectedSwiActions;
        }

        public static Control GetPostBackControl(Page page)
        {
            Control control = null;
            string ctrlname = page.Request.Params.Get("__EVENTTARGET");
            if (!string.IsNullOrEmpty(ctrlname))
            {
                control = page.FindControl(ctrlname);
            }
            else
            {
                foreach (string ctl in page.Request.Form)
                {
                    Control c = page.FindControl(ctl);
                    if (c is System.Web.UI.WebControls.Button)
                    {
                        control = c;
                        break;
                    }
                }

            }

            return control;
        }
    }
}