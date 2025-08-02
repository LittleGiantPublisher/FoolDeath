using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


namespace PlatformSupport
{

#if UNITY_PS4 
    public class PS4SaveData
    {
        const string kMountDirName = "saves";
        const ulong kTotalBlocksSize = Sony.PS4.SaveData.Mounting.MountRequest.BLOCKS_MIN + 360; // aqui e o tamanho, esses blocos são os bytes, aqui ta um padrão de 12mb eu acho, mas pode aumentar e ver se resolve, acho que nao e isso mas sla oq pode ser

        public int m_userID { get; private set; }
        MonoBehaviour _dispatcher;
        Sony.PS4.SaveData.InitResult m_initResult;
        List<Sony.PS4.SaveData.SaveDataCallbackEvent> m_saveDataEventsToDispatch = new List<Sony.PS4.SaveData.SaveDataCallbackEvent>(4);
        bool waitingResponse;
        Sony.PS4.SaveData.ReturnCodes operationResult;
        ulong m_requiredFreeBlocks;

        public bool isSaving { get; internal set; }
        public bool isLoading{ get; internal set; }

        public void Init(int userID, MonoBehaviour aRoutineDispatcher)
        {
            m_userID = userID;
            _dispatcher = aRoutineDispatcher;

            Sony.PS4.SaveData.Main.OnAsyncEvent += OnPS4Event_SaveDataAsyncEvent;

            try
            {
                Sony.PS4.SaveData.InitSettings settings = new Sony.PS4.SaveData.InitSettings();

                settings.Affinity = Sony.PS4.SaveData.ThreadAffinity.Core5;

                m_initResult = Sony.PS4.SaveData.Main.Initialize(settings);

                if (!m_initResult.Initialized)
                {
                   //Debug.LogError("[SaveDataTask] not initialized ");
                }
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                //Debug.LogError("Exception During Initialization : " + e.ExtendedMessage);
            }
        }

        public void Finish()
        {
            //Sony.PS4.SaveData.Main.Terminate();

            m_initResult = new Sony.PS4.SaveData.InitResult();
        }

        public IEnumerator CommitSave(byte[] data, string fileName, System.Action<DataResult> resultCallback)
        {
            //if (SaveLoad.commitCount > 0) { //Debug.LogError("EU PAREI AQUI: " + fileName); yield break; }
            //SaveLoad.commitCount += 1;
            //Debug.Log("ps4CommitCount: " + SaveLoad.commitCount);

            var opResult = DataResult.Success;

            isSaving = true;

            yield return null;

            operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_BUSY;
            waitingResponse = true;
            Mount(readWrite: true);
            yield return _dispatcher.StartCoroutine(WaitResponse());

            // check Mount async result
            switch (operationResult)
            {
                case Sony.PS4.SaveData.ReturnCodes.SUCCESS:
                    Porting.PlatformManager.notSpaceAvailable = false;
                    break;

                case Sony.PS4.SaveData.ReturnCodes.DATA_ERROR_NO_SPACE_FS:
                    {
                        operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_BUSY;
                        waitingResponse = true;
                        DisplayNoFreeSpaceDialog(m_requiredFreeBlocks);
                        yield return _dispatcher.StartCoroutine(WaitResponse());
                        Porting.PlatformManager.notSpaceAvailable = true;
                        resultCallback?.Invoke(DataResult.FailedNoFreeSpace);
                        yield break;
                    }
                    break;

                case Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_BROKEN:
                    {
                        operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_BUSY;
                        waitingResponse = true;
                        DisplayBrokenDataDeletedDialog(Sony.PS4.SaveData.Dialogs.DialogType.Save, Sony.PS4.SaveData.Dialogs.SystemMessageType.CorruptedAndCreate);
                        yield return _dispatcher.StartCoroutine(WaitResponse());

                        waitingResponse = true;
                        Delete();
                        yield return _dispatcher.StartCoroutine(WaitResponse());

                        // check delete async result
                        if (operationResult != Sony.PS4.SaveData.ReturnCodes.SUCCESS)
                        {
                            waitingResponse = true;
                            DisplayErrorDialog(Sony.PS4.SaveData.Dialogs.DialogType.Save, unchecked((int)operationResult));
                            yield return _dispatcher.StartCoroutine(WaitResponse());

                            resultCallback?.Invoke(DataResult.GenericError);
                            yield break;
                        }

                        _dispatcher.StartCoroutine(CommitSave(data, fileName, resultCallback));
                        yield break;
                    }
                    break;

                default:
                    {
                        waitingResponse = true;
                        DisplayErrorDialog(Sony.PS4.SaveData.Dialogs.DialogType.Save, unchecked((int)operationResult));
                        yield return _dispatcher.StartCoroutine(WaitResponse());

                        resultCallback?.Invoke(DataResult.GenericError);
                        yield break;
                    }
                    break;
            }

            // Acquire the active Mount point
            var mp = Sony.PS4.SaveData.Mounting.ActiveMountPoints[0];

            waitingResponse = true;
            WriteData(data, fileName, mp);
            yield return _dispatcher.StartCoroutine(WaitResponse());

            //Debug.LogError("Estou checanto se houve erro na escrita : " + fileName);
            // check WriteData async result
            switch (operationResult)
            {
                case Sony.PS4.SaveData.ReturnCodes.SUCCESS:
                    //Debug.LogError("Passei pela escrita: " + fileName);
                    break;

                default:
                    {
                        waitingResponse = true;
                        DisplayErrorDialog(Sony.PS4.SaveData.Dialogs.DialogType.Save, unchecked((int)operationResult));
                        yield return _dispatcher.StartCoroutine(WaitResponse());

                        opResult = DataResult.GenericError;
                    }
                    break;
            }

            waitingResponse = true;
            Unmount(mp, backup: true);
            yield return _dispatcher.StartCoroutine(WaitResponse());

            //Debug.LogError("Estou checanto se houve erro no Unmount: " + fileName);
            // check Unmount async result
            if (operationResult != Sony.PS4.SaveData.ReturnCodes.SUCCESS)
            {
                waitingResponse = true;
                DisplayErrorDialog(Sony.PS4.SaveData.Dialogs.DialogType.Save, unchecked((int)operationResult));
                yield return _dispatcher.StartCoroutine(WaitResponse());

                resultCallback?.Invoke(DataResult.GenericError);
                yield break;
            }
            //Debug.LogError("Passei pelo Unmount: " + fileName);
            // wait for backup async result
            waitingResponse = true;
            yield return _dispatcher.StartCoroutine(WaitResponse());

            //Debug.LogError("Checando Backup: " + fileName);
            // check backup async result
            if (operationResult != Sony.PS4.SaveData.ReturnCodes.SUCCESS)
            {
                waitingResponse = true;
                DisplayErrorDialog(Sony.PS4.SaveData.Dialogs.DialogType.Save, unchecked((int)operationResult));
                yield return _dispatcher.StartCoroutine(WaitResponse());
            }
            //Debug.LogError("Passei pelo Backup: " + fileName);
            // done
            resultCallback?.Invoke(opResult);
            //Debug.LogError("Conclui o save: " + fileName);
        }

        public IEnumerator LoadRequest(ScriptableObject SO,string fileName, System.Action<DataResult, byte[], string, ScriptableObject> resultCallback)
        {
            var opResult = DataResult.Success;

            isLoading = true;
            Debug.Log($"[Load_Slot] LoadRequest filename: {fileName}");

            yield return null;

            operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_BUSY;
            waitingResponse = true;
            Mount(readWrite: false);
            yield return _dispatcher.StartCoroutine(WaitResponse());

            // check Mount async result
            switch (operationResult)
            {
                case Sony.PS4.SaveData.ReturnCodes.SUCCESS:
                    break;

                case Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_NOT_FOUND:
                    {
                        resultCallback?.Invoke(DataResult.DoesntExists, null, fileName, SO);
                        yield break;
                    }

                     break;

                case Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_BROKEN:
                    {
                        operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_BUSY;
                        
                        waitingResponse = true;
                        CheckBackup();
                        yield return _dispatcher.StartCoroutine(WaitResponse());

                        // CheckBackup async result
                        switch (operationResult)
                        {
                            case Sony.PS4.SaveData.ReturnCodes.SUCCESS:
                                {
                                    waitingResponse = true;
                                    DisplayBrokenDataRestoreDialog();
                                    yield return _dispatcher.StartCoroutine(WaitResponse());

                                    waitingResponse = true;
                                    RestoreBackup();
                                    yield return _dispatcher.StartCoroutine(WaitResponse());

                                    // check restore backup async result
                                    if (operationResult == Sony.PS4.SaveData.ReturnCodes.DATA_ERROR_NO_SPACE_FS)
                                    {
                                        waitingResponse = true;
                                        DisplayNoFreeSpaceRestoreDialog();
                                        yield return _dispatcher.StartCoroutine(WaitResponse());

                                        resultCallback?.Invoke(DataResult.GenericError, null, fileName, SO);
                                        yield break;
                                    }

                                    else if (operationResult != Sony.PS4.SaveData.ReturnCodes.SUCCESS)
                                    {
                                        waitingResponse = true;
                                        DisplayErrorDialog(Sony.PS4.SaveData.Dialogs.DialogType.Save, unchecked((int)operationResult));
                                        yield return _dispatcher.StartCoroutine(WaitResponse());

                                        resultCallback?.Invoke(DataResult.DoesntExists, null, fileName,SO);
                                        yield break;
                                    }

                                    // redo process with clean restored area
                                    //SaveLoad.loadCount -= 1;
                                    _dispatcher.StartCoroutine(LoadRequest(SO,fileName, resultCallback));
                                    yield break;

                                }
                                break;

                            case Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_NOT_FOUND:
                                {
                                    waitingResponse = true;
                                    DisplayBrokenDataDeletedDialog(Sony.PS4.SaveData.Dialogs.DialogType.Load, Sony.PS4.SaveData.Dialogs.SystemMessageType.CorruptedAndDelete);
                                    yield return _dispatcher.StartCoroutine(WaitResponse());
                                    
                                    waitingResponse = true;
                                    Delete();
                                    yield return _dispatcher.StartCoroutine(WaitResponse());

                                    // check delete async result
                                    if (operationResult != Sony.PS4.SaveData.ReturnCodes.SUCCESS)
                                    {
                                        waitingResponse = true;
                                        DisplayErrorDialog(Sony.PS4.SaveData.Dialogs.DialogType.Save, unchecked((int)operationResult));
                                        yield return _dispatcher.StartCoroutine(WaitResponse());

                                        resultCallback?.Invoke(DataResult.DoesntExists, null, fileName,SO);
                                        yield break;
                                    }

                                    // finishes as SAVE_DATA_ERROR_NOT_FOUND
                                    resultCallback?.Invoke(DataResult.DoesntExists, null, fileName,SO);
                                    yield break;
                                }
                                break;

                            default:
                                waitingResponse = true;
                                DisplayErrorDialog(Sony.PS4.SaveData.Dialogs.DialogType.Load, unchecked((int)operationResult));
                                yield return _dispatcher.StartCoroutine(WaitResponse());

                                resultCallback?.Invoke(DataResult.DoesntExists, null, fileName,SO);
                                yield break;

                        }
                    }
                    break;

                default:
                    {
                        waitingResponse = true;
                        DisplayErrorDialog(Sony.PS4.SaveData.Dialogs.DialogType.Load, unchecked((int)operationResult));
                        yield return _dispatcher.StartCoroutine(WaitResponse());

                        resultCallback?.Invoke(DataResult.DoesntExists, null, fileName,SO);
                        yield break;
                    }
                    break;
            }

            // Acquire the active Mount point
            var mp = Sony.PS4.SaveData.Mounting.ActiveMountPoints[0];

            byte[] data = null;

            waitingResponse = true;
            var readResponse = ReadData(mp, fileName);
            yield return _dispatcher.StartCoroutine(WaitResponse());

            if (readResponse != null)
            {
                // check ReadData async result
                switch (operationResult)
                {
                    case Sony.PS4.SaveData.ReturnCodes.SUCCESS:
                        {
                            data = readResponse.Data;
                        }
                        break;

                    default:
                        {
                            waitingResponse = true;
                            DisplayErrorDialog(Sony.PS4.SaveData.Dialogs.DialogType.Save, unchecked((int)operationResult));
                            yield return _dispatcher.StartCoroutine(WaitResponse());

                            opResult = DataResult.DoesntExists;
                        }
                        break;
                }
            }

            waitingResponse = true;
            Unmount(mp, backup: true);
            yield return _dispatcher.StartCoroutine(WaitResponse());

            // check Unmount async result
            if (operationResult != Sony.PS4.SaveData.ReturnCodes.SUCCESS)
            {
                waitingResponse = true;
                DisplayErrorDialog(Sony.PS4.SaveData.Dialogs.DialogType.Load, unchecked((int)operationResult));
                yield return _dispatcher.StartCoroutine(WaitResponse());

                resultCallback?.Invoke(DataResult.DoesntExists, null, fileName,SO);
                yield break;
            }

            // done
            resultCallback?.Invoke(opResult, data, fileName,SO);
        }

        IEnumerator WaitResponse()
        {
            while (waitingResponse)
            {
                yield return null;
            }
        }

        void Mount(bool readWrite)
        {
            try
            {
                Sony.PS4.SaveData.Mounting.MountRequest request = new Sony.PS4.SaveData.Mounting.MountRequest();

                Sony.PS4.SaveData.DirName dirName = new Sony.PS4.SaveData.DirName();
                dirName.Data = kMountDirName;

                request.UserId = m_userID;
                request.Async = true;
                request.DirName = dirName;

                if (readWrite == true)
                {
                    request.MountMode = Sony.PS4.SaveData.Mounting.MountModeFlags.Create2 |
                                        Sony.PS4.SaveData.Mounting.MountModeFlags.CopyIcon |
                                        Sony.PS4.SaveData.Mounting.MountModeFlags.ReadWrite;

                    request.Blocks = kTotalBlocksSize;
                }
                else
                {
                    request.MountMode = Sony.PS4.SaveData.Mounting.MountModeFlags.ReadOnly;
                }

                Sony.PS4.SaveData.Mounting.MountResponse response = new Sony.PS4.SaveData.Mounting.MountResponse();

                int requestID = Sony.PS4.SaveData.Mounting.Mount(request, response);
                ////Debug.Log("[SaveDataTask] Requested Mount : " + requestID);
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                //////Debug.LogError("[SaveDataTask] Mount Exception : " + e.ExtendedMessage);
                waitingResponse = false;
                operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_NOT_MOUNTED;
            }
        }


        void Unmount(Sony.PS4.SaveData.Mounting.MountPoint mp, bool backup)
        {
            try
            {
                Sony.PS4.SaveData.Mounting.UnmountRequest request = new Sony.PS4.SaveData.Mounting.UnmountRequest();

                if (mp == null) { 
                                //Debug.LogError("MP está Nulo");
                                  return; }

                request.UserId = m_userID;
                request.MountPointName = mp.PathName;
                request.Backup = backup;

                Sony.PS4.SaveData.EmptyResponse response = new Sony.PS4.SaveData.EmptyResponse();

                int requestID = Sony.PS4.SaveData.Mounting.Unmount(request, response);
                ////Debug.Log("[SaveDataTask] Requested Unmount : " + requestID);
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                //////Debug.LogError("[SaveDataTask] Unmount Exception : " + e.ExtendedMessage);
                waitingResponse = false;
                operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_INTERNAL;
            }
        }


        void WriteData(byte[] saveData, string fileName, Sony.PS4.SaveData.Mounting.MountPoint mp)
        {
            try
            {
                // Parameters to use for the savedata
                Sony.PS4.SaveData.SaveDataParams saveDataParams = new Sony.PS4.SaveData.SaveDataParams();

                saveDataParams.Title = "Save Data";
                saveDataParams.SubTitle = "";
                saveDataParams.Detail = "";

                PS4FileWriteOperationRequest request = new PS4FileWriteOperationRequest();

                request.UserId = m_userID;
                request.MountPointName = mp.PathName;
                request.Data = saveData;
                request.fileName = fileName;

                PS4FileWriteOperationResponse response = new PS4FileWriteOperationResponse();

                int requestID = Sony.PS4.SaveData.FileOps.CustomFileOp(request, response);
                ////Debug.Log("[SaveDataTask] Requested CustomFileOp Write : " + requestID);
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                //////Debug.LogError("[SaveDataTask] WriteData Exception : " + e.ExtendedMessage);
                waitingResponse = false;
                operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_INTERNAL;
            }
        }


        PS4FileReadOperationResponse ReadData(Sony.PS4.SaveData.Mounting.MountPoint mp, string fileName)
        {
            try
            {
                PS4FileReadOperationRequest request = new PS4FileReadOperationRequest();

                request.UserId = m_userID;
                request.MountPointName = mp.PathName;
                request.fileName = fileName;

                Debug.Log($"[Load_Slots] MountPointName {mp.PathName} " +
                    $"\nFilename {fileName} " +
                    $"\nUserId {m_userID}");

                PS4FileReadOperationResponse response = new PS4FileReadOperationResponse();

                int requestID = Sony.PS4.SaveData.FileOps.CustomFileOp(request, response);
                ////Debug.Log("[SaveDataTask] Requested CustomFileOp Read : " + requestID);

                return response;
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                //////Debug.LogError("[SaveDataTask] ReadData Exception : " + e.ExtendedMessage);
                waitingResponse = false;
                operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_INTERNAL;
            }
            return null;
        }


        private void Delete()
        {
            try
            {
                Sony.PS4.SaveData.Deleting.DeleteRequest request = new Sony.PS4.SaveData.Deleting.DeleteRequest();

                Sony.PS4.SaveData.DirName dirName = new Sony.PS4.SaveData.DirName();
                dirName.Data = kMountDirName;

                request.UserId = m_userID;
                request.DirName = dirName;

                Sony.PS4.SaveData.EmptyResponse response = new Sony.PS4.SaveData.EmptyResponse();

                int requestID = Sony.PS4.SaveData.Deleting.Delete(request, response);
                ////Debug.Log("[SaveDataTask] Requested Delete : " + requestID);
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                ////Debug.LogException(e);
                waitingResponse = false;
                operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_INTERNAL;
            }
        }


        void CheckBackup()
        {
            try
            {
                Sony.PS4.SaveData.Backups.CheckBackupRequest request = new Sony.PS4.SaveData.Backups.CheckBackupRequest();

                Sony.PS4.SaveData.DirName dirName = new Sony.PS4.SaveData.DirName();
                dirName.Data = kMountDirName;

                request.UserId = m_userID;
                request.DirName = dirName;
                request.IncludeParams = false;
                request.IncludeIcon = false;

                Sony.PS4.SaveData.Backups.CheckBackupResponse response = new Sony.PS4.SaveData.Backups.CheckBackupResponse();

                int requestID = Sony.PS4.SaveData.Backups.CheckBackup(request, response);
                ////Debug.Log("[SaveDataTask] Requested CheckBackup : " + requestID);
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                ////Debug.LogException(e);
                waitingResponse = false;
                operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_INTERNAL;
            }
        }


        void RestoreBackup()
        {
            try
            {
                Sony.PS4.SaveData.Backups.RestoreBackupRequest request = new Sony.PS4.SaveData.Backups.RestoreBackupRequest();

                Sony.PS4.SaveData.DirName dirName = new Sony.PS4.SaveData.DirName();
                dirName.Data = kMountDirName;

                request.UserId = m_userID;
                request.DirName = dirName;

                Sony.PS4.SaveData.EmptyResponse response = new Sony.PS4.SaveData.EmptyResponse();

                int requestID = Sony.PS4.SaveData.Backups.RestoreBackup(request, response);
                ////Debug.Log("[SaveDataTask] Requested RestoreBackup : " + requestID);
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                ////Debug.LogException(e);
                waitingResponse = false;
                operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_INTERNAL;
            }
        }


        void DisplayNoFreeSpaceDialog(ulong requiredFreeBlocks)
        {
            try
            {
                Sony.PS4.SaveData.Dialogs.OpenDialogRequest request = new Sony.PS4.SaveData.Dialogs.OpenDialogRequest();

                request.UserId = m_userID;
                request.Mode = Sony.PS4.SaveData.Dialogs.DialogMode.SystemMsg;
                request.DispType = Sony.PS4.SaveData.Dialogs.DialogType.Save;

                request.Animations = new Sony.PS4.SaveData.Dialogs.AnimationParam(Sony.PS4.SaveData.Dialogs.Animation.On, Sony.PS4.SaveData.Dialogs.Animation.On);

                Sony.PS4.SaveData.Dialogs.SystemMessageParam msg = new Sony.PS4.SaveData.Dialogs.SystemMessageParam();
                msg.SysMsgType = Sony.PS4.SaveData.Dialogs.SystemMessageType.NoSpaceContinuable;
                msg.Value = requiredFreeBlocks;

                request.SystemMessage = msg;

                Sony.PS4.SaveData.Dialogs.OpenDialogResponse response = new Sony.PS4.SaveData.Dialogs.OpenDialogResponse();

                int requestID = Sony.PS4.SaveData.Dialogs.OpenDialog(request, response);
                ////Debug.Log("[SaveDataTask] Requested Dialog NoSpaceContinuable : " + requestID);
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                ////Debug.LogException(e);
                waitingResponse = false;
                operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_INTERNAL;
            }
        }


        private void DisplayNoFreeSpaceRestoreDialog()
        {
            try
            {
                Sony.PS4.SaveData.Dialogs.OpenDialogRequest request = new Sony.PS4.SaveData.Dialogs.OpenDialogRequest();

                request.UserId = m_userID;
                request.Mode = Sony.PS4.SaveData.Dialogs.DialogMode.SystemMsg;
                request.DispType = Sony.PS4.SaveData.Dialogs.DialogType.Load;

                request.Animations = new Sony.PS4.SaveData.Dialogs.AnimationParam(Sony.PS4.SaveData.Dialogs.Animation.On, Sony.PS4.SaveData.Dialogs.Animation.On);

                Sony.PS4.SaveData.Dialogs.SystemMessageParam msg = new Sony.PS4.SaveData.Dialogs.SystemMessageParam();
                msg.SysMsgType = (Sony.PS4.SaveData.Dialogs.SystemMessageType)15; // SCE_SAVE_DATA_DIALOG_SYSMSG_TYPE_NOSPACE_RESTORE
                //msg.Value = requiredFreeBlocks;

                request.SystemMessage = msg;

                // Required free blocks will be automatically calculated by the system based on DirName
                Sony.PS4.SaveData.DirName[] dirNames = new Sony.PS4.SaveData.DirName[1];
                dirNames[0] = new Sony.PS4.SaveData.DirName() { Data = kMountDirName };

                Sony.PS4.SaveData.Dialogs.Items items = new Sony.PS4.SaveData.Dialogs.Items();
                items.DirNames = dirNames;

                request.Items = items;

                Sony.PS4.SaveData.Dialogs.OpenDialogResponse response = new Sony.PS4.SaveData.Dialogs.OpenDialogResponse();

                int requestID = Sony.PS4.SaveData.Dialogs.OpenDialog(request, response);
                ////Debug.Log("[SaveDataTask] Requested Dialog NoSpaceRestore : " + requestID);
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                ////Debug.LogException(e);
                waitingResponse = false;
                operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_INTERNAL;
            }
        }

        private void DisplayBrokenDataDeletedDialog(Sony.PS4.SaveData.Dialogs.DialogType dialogType, Sony.PS4.SaveData.Dialogs.SystemMessageType systemMessage)
        {
            try
            {
                Sony.PS4.SaveData.Dialogs.OpenDialogRequest request = new Sony.PS4.SaveData.Dialogs.OpenDialogRequest();

                request.UserId = m_userID;
                request.Mode = Sony.PS4.SaveData.Dialogs.DialogMode.SystemMsg;
                request.DispType = dialogType;

                request.Animations = new Sony.PS4.SaveData.Dialogs.AnimationParam(Sony.PS4.SaveData.Dialogs.Animation.On, Sony.PS4.SaveData.Dialogs.Animation.On);

                Sony.PS4.SaveData.Dialogs.SystemMessageParam msg = new Sony.PS4.SaveData.Dialogs.SystemMessageParam();
                msg.SysMsgType = systemMessage;

                Sony.PS4.SaveData.DirName[] dirNames = new Sony.PS4.SaveData.DirName[1];
                dirNames[0] = new Sony.PS4.SaveData.DirName() { Data = kMountDirName };

                Sony.PS4.SaveData.Dialogs.Items items = new Sony.PS4.SaveData.Dialogs.Items();
                items.DirNames = dirNames;

                request.Items = items;

                Sony.PS4.SaveData.Dialogs.OptionParam optionParam = new Sony.PS4.SaveData.Dialogs.OptionParam();
                optionParam.Back = Sony.PS4.SaveData.Dialogs.OptionBack.Disable;

                request.Option = optionParam;

                request.SystemMessage = msg;

                Sony.PS4.SaveData.Dialogs.OpenDialogResponse response = new Sony.PS4.SaveData.Dialogs.OpenDialogResponse();

                int requestID = Sony.PS4.SaveData.Dialogs.OpenDialog(request, response);
                ////Debug.Log("[SaveDataTask] Requested Dialog CorruptedAndCreate : " + requestID);
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                ////Debug.LogException(e);
                waitingResponse = false;
                operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_INTERNAL;
            }
        }

        private void DisplayBrokenDataRestoreDialog()
        {
            try
            {
                Sony.PS4.SaveData.Dialogs.OpenDialogRequest request = new Sony.PS4.SaveData.Dialogs.OpenDialogRequest();

                request.UserId = m_userID;
                request.Mode = Sony.PS4.SaveData.Dialogs.DialogMode.SystemMsg;
                request.DispType = Sony.PS4.SaveData.Dialogs.DialogType.Load;

                request.Animations = new Sony.PS4.SaveData.Dialogs.AnimationParam(Sony.PS4.SaveData.Dialogs.Animation.On, Sony.PS4.SaveData.Dialogs.Animation.On);

                Sony.PS4.SaveData.Dialogs.SystemMessageParam msg = new Sony.PS4.SaveData.Dialogs.SystemMessageParam();
                msg.SysMsgType = Sony.PS4.SaveData.Dialogs.SystemMessageType.CurruptedAndRestore;

                Sony.PS4.SaveData.DirName[] dirNames = new Sony.PS4.SaveData.DirName[1];
                dirNames[0] = new Sony.PS4.SaveData.DirName() { Data = kMountDirName };

                Sony.PS4.SaveData.Dialogs.Items items = new Sony.PS4.SaveData.Dialogs.Items();
                items.DirNames = dirNames;

                request.Items = items;

                Sony.PS4.SaveData.Dialogs.OptionParam optionParam = new Sony.PS4.SaveData.Dialogs.OptionParam();
                optionParam.Back = Sony.PS4.SaveData.Dialogs.OptionBack.Disable;

                request.Option = optionParam;

                request.SystemMessage = msg;

                Sony.PS4.SaveData.Dialogs.OpenDialogResponse response = new Sony.PS4.SaveData.Dialogs.OpenDialogResponse();

                int requestID = Sony.PS4.SaveData.Dialogs.OpenDialog(request, response);
                ////Debug.Log("[SaveDataTask] Requested Dialog CurruptedAndRestore : " + requestID);
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                ////Debug.LogException(e);
                waitingResponse = false;
                operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_INTERNAL;
            }
        }

        private void DisplayErrorDialog(Sony.PS4.SaveData.Dialogs.DialogType dispType, int errorCode)
        {
            try
            {
                Sony.PS4.SaveData.Dialogs.OpenDialogRequest request = new Sony.PS4.SaveData.Dialogs.OpenDialogRequest();

                request.UserId = m_userID;
                request.Mode = Sony.PS4.SaveData.Dialogs.DialogMode.ErrorCode;
                request.DispType = dispType;

                request.Animations = new Sony.PS4.SaveData.Dialogs.AnimationParam(Sony.PS4.SaveData.Dialogs.Animation.On, Sony.PS4.SaveData.Dialogs.Animation.On);

                Sony.PS4.SaveData.Dialogs.ErrorCodeParam errorParam = new Sony.PS4.SaveData.Dialogs.ErrorCodeParam();
                errorParam.ErrorCode = errorCode;

                request.ErrorCode = errorParam;

                Sony.PS4.SaveData.Dialogs.OpenDialogResponse response = new Sony.PS4.SaveData.Dialogs.OpenDialogResponse();

                int requestID = Sony.PS4.SaveData.Dialogs.OpenDialog(request, response);
                ////Debug.Log("[SaveDataTask] Requested Dialog ErrorDialog : " + requestID);
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                ////Debug.LogException(e);
                waitingResponse = false;
                operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_INTERNAL;
            }
        }

        private void OnPS4Event_SaveDataAsyncEvent(Sony.PS4.SaveData.SaveDataCallbackEvent callbackEvent)
        {
            // Enquee to dispatch at the Main Thread
            m_saveDataEventsToDispatch.Add(callbackEvent);
        }

        public void UpdateState()
        {
            if (m_saveDataEventsToDispatch.Count > 0)
            {
                var callback = m_saveDataEventsToDispatch[0];
                m_saveDataEventsToDispatch.RemoveAt(0);
                OnPS4Event_SaveDataEventDispatcher(callback);
            }
        }

        void OnPS4Event_SaveDataEventDispatcher(Sony.PS4.SaveData.SaveDataCallbackEvent callbackEvent)
        {
            ////Debug.Log("[PS4 SaveData] API Called = (" + callbackEvent.ApiCalled + ") : Request Id = (" + callbackEvent.RequestId + ") : Calling User Id = (0x" + callbackEvent.UserId.ToString("X8") + ")");

            try
            {
                if (callbackEvent.Response != null)
                {
                    if (callbackEvent.Response.ReturnCodeValue < 0)
                    {
                        //////Debug.LogError("[SaveDataTask] Error Response : " + callbackEvent.Response.ConvertReturnCodeToString(callbackEvent.ApiCalled));
                    }
                    else
                    {
                        ////Debug.Log("[SaveDataTask] Response : " + callbackEvent.Response.ConvertReturnCodeToString(callbackEvent.ApiCalled));
                    }

                    if (callbackEvent.Response.Exception != null)
                    {
                        if (callbackEvent.Response.Exception is Sony.PS4.SaveData.SaveDataException)
                        {
                            //////Debug.LogError("[SaveDataTask] Response Exception: " + ((Sony.PS4.SaveData.SaveDataException)callbackEvent.Response.Exception).ExtendedMessage);
                        }
                        else
                        {
                            //////Debug.LogError("[SaveDataTask] Response Exception: " + callbackEvent.Response.Exception.Message);
                        }
                    }
                }

                // Dispatch event
                switch (callbackEvent.ApiCalled)
                {
                    case Sony.PS4.SaveData.FunctionTypes.Mount:
                        {
                            var response = callbackEvent.Response as Sony.PS4.SaveData.Mounting.MountResponse;

                            if (response != null)
                            {
                                m_requiredFreeBlocks = response.RequiredBlocks;
                                operationResult = response.ReturnCode;
                            }

                            waitingResponse = false;
                        }
                        break;
                    case Sony.PS4.SaveData.FunctionTypes.Unmount:
                    case Sony.PS4.SaveData.FunctionTypes.Delete:
                    case Sony.PS4.SaveData.FunctionTypes.NotificationUnmountWithBackup:
                    case Sony.PS4.SaveData.FunctionTypes.FileOps:
                    case Sony.PS4.SaveData.FunctionTypes.CheckBackup:
                    case Sony.PS4.SaveData.FunctionTypes.RestoreBackup:
                        {
                            if (callbackEvent.Response != null)
                            {
                                operationResult = callbackEvent.Response.ReturnCode;
                            }

                            waitingResponse = false;
                        }
                        break;
                    case Sony.PS4.SaveData.FunctionTypes.OpenDialog:
                        {
                            Debug.Log($"Open DIalog Response");
                            Debug.Log($"Request FunctionType = {callbackEvent.Request.FunctionType}");

                            var response = callbackEvent.Response as Sony.PS4.SaveData.Dialogs.OpenDialogResponse;

                            Sony.PS4.SaveData.Dialogs.DialogResult result = response.Result;

                            if (result == null)
                            {
                                Debug.Log("Error occured when opening dialog");
                                waitingResponse = false;
                                return;
                            }

                            if (result.CallResult == Sony.PS4.SaveData.Dialogs.DialogCallResults.OK)
                            {
                                Debug.Log($"Call Result is OK");
                                //operationResult = Sony.PS4.SaveData.ReturnCodes.SUCCESS;
                            }
                            else
                            {
                                Debug.Log($"Call Result is Cancel");
                                //operationResult = Sony.PS4.SaveData.ReturnCodes.SAVE_DATA_ERROR_INTERNAL;
                            }

                            operationResult = Sony.PS4.SaveData.ReturnCodes.SUCCESS;
                            waitingResponse = false;
                        }
                        break;
                }
            }
            catch (Sony.PS4.SaveData.SaveDataException e)
            {
                //////Debug.LogError("OnPS4Event_SaveDataEventDispatcher SaveData Exception = " + e.ExtendedMessage);
            }
            catch (System.Exception e)
            {
                ////Debug.LogException(e);
            }
        }
    }


        class PS4FileWriteOperationRequest : Sony.PS4.SaveData.FileOps.FileOperationRequest
    {
        public byte[] Data;
        public string fileName;

        public override void DoFileOperations(Sony.PS4.SaveData.Mounting.MountPoint mp, Sony.PS4.SaveData.FileOps.FileOperationResponse response)
        {
            string filePath = string.Format("{0}/{1}", mp.PathName.Data, fileName);

            int totalWritten = 0;

            ////Debug.Log("[SaveDataTask] Creating save file with size: " + Data.LongLength);

            using (FileStream fs = File.Open(filePath, FileMode.Create))
            {
                while (totalWritten < Data.Length)
                {
                    int writeSize = Mathf.Min(Data.Length - totalWritten, 1000); // Write up to 1000 bytes

                    fs.Write(Data, totalWritten, writeSize);

                    totalWritten += writeSize;

                    // Update progress value during saving
                    response.UpdateProgress((float)totalWritten / (float)Data.Length);
                }
            }

            ////Debug.Log("[SaveDataTask] Total written: " + totalWritten);
        }
    }

    class PS4FileReadOperationRequest : Sony.PS4.SaveData.FileOps.FileOperationRequest
    {
        public string fileName;

        public override void DoFileOperations(Sony.PS4.SaveData.Mounting.MountPoint mp, Sony.PS4.SaveData.FileOps.FileOperationResponse response)
        {
            var readResponse = response as PS4FileReadOperationResponse;

            string filePath = string.Format("{0}/{1}", mp.PathName.Data, fileName);

            FileInfo info = new FileInfo(filePath);

            readResponse.Data = new byte[info.Length];

            int totalRead = 0;

            using (FileStream fs = File.OpenRead(filePath))
            {
                while (totalRead < info.Length)
                {
                    int readSize = Mathf.Min((int)info.Length - totalRead, 1000); // read up to 1000 bytes

                    fs.Read(readResponse.Data, totalRead, readSize);

                    totalRead += readSize;

                    // Update progress value during loading
                    response.UpdateProgress((float)totalRead / (float)info.Length);
                }
            }

            ////Debug.Log("[SaveDataTask] Total read: " + totalRead);
        }
    }

    class PS4FileWriteOperationResponse : Sony.PS4.SaveData.FileOps.FileOperationResponse
    {

    }

    class PS4FileReadOperationResponse : Sony.PS4.SaveData.FileOps.FileOperationResponse
    {
        public byte[] Data;
    }
#endif
}
// namespace PlatformSupport


