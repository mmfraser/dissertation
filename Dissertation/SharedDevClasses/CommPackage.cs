
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace ComputeAndroidSDK.Communication {
    [Serializable]
    public class CommPackage {
        public int ComputationRequestId {
            get;
            set;
        }

        public int DeviceLocalRequestId {
            get;
            set;
        }

        public String IntentAction {
            get;
            set;
        }
        public String BackgroundProcessFunction {
            get;
            set;
        }
        public String BackgroundProcessClass {
            get;
            set;
        }
        public List<ParamListItem> ParameterList {
            get;
            set;
        }
        public String ComputationResult {
            get;
            set;
        }
        public int ApplicationId {
            get;
            set;
        }
        public DateTime ComputationStartTime {
            get;
            set;
        }
        public DateTime ComputationEndTime {
            get;
            set;
        }

        public String DeviceUIRef {
            get;
            set;
        }

        public decimal? SerialisationTime {
            get;
            set;
        }

        public decimal? DeserialisationTime {
            get;
            set;
        }

        public String SerializeParamList() {
            return JsonConvert.SerializeObject(ParameterList);
        }

        public static List<ParamListItem> DeserializeParamJson(String json) {
            return JsonConvert.DeserializeObject<List<ParamListItem>>(json);
        }

        public String SerializeJson() {
            return JsonConvert.SerializeObject(this);
        }

        public static CommPackage DeserializeJson(String json) {


            return JsonConvert.DeserializeObject<CommPackage>(json);
        }

        [Serializable]
        public class ParamListItem {
            public String ParameterName {
                get;
                set;
            }
            public Object ParameterValue {
                get;
                set;
            }

            public ParamListItem(String _paramName, Object _paramValue) {
                this.ParameterName = _paramName;
                this.ParameterValue = _paramValue;
            }

        }
    }




}