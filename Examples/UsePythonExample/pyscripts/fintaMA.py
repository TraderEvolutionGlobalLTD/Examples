
import pandas as pd
from finta import TA
import json

class MovingAvarageModel:
       

    def getMA(self, json_arguments):

        de_serialized_args = json.loads(json_arguments)

        data_file = de_serialized_args["model_input"] 

        period =  de_serialized_args["period"]   
        
        raw_data =  json.dumps(data_file)

        data = pd.read_json(raw_data, orient=["time"]).set_index("time")

        ma = TA.SMA(data, period)

        return_obj = {
            "result" : ma.values.tolist()
        }

        return json.dumps(return_obj)