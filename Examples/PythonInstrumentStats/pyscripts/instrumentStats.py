
import pandas as pd
import numpy as np
import json

class InstrumentStatsModel:
       

    def getStats(self, json_arguments):

        de_serialized_args = json.loads(json_arguments)

        data_file = de_serialized_args["model_input"] 

        csv_file = de_serialized_args["csv_path"] 
        
        raw_data =  json.dumps(data_file)

        df= pd.read_csv(csv_file)

        model_input = pd.read_json(raw_data)

        if hasattr(model_input, 'symbol'):
            result_indices = np.where(df["Instrument Symbol"] == model_input.symbol)       

        #return_obj = {
            #"result" : df.iloc[result_indices].tolist()
        #}

        return_obj = {
            "result" : model_input#df["Instrument Symbol"].tolist()
        }

        return json.dumps(return_obj)