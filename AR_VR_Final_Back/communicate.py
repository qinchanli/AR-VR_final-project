import time
import zmq
from transformers import AutoModelForCausalLM, AutoTokenizer
import torch
import os
import json
from flair.data import Sentence
from flair.models import SequenceTagger
from sentence_transformers import SentenceTransformer
from sklearn.metrics.pairwise import cosine_similarity

tokenizer = AutoTokenizer.from_pretrained("microsoft/DialoGPT-large")
model_regular = AutoModelForCausalLM.from_pretrained("microsoft/DialoGPT-large")

model_segmentation = SentenceTransformer('bert-base-nli-mean-tokens')
tagger = SequenceTagger.load("flair/chunk-english")

step = 0
context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind('tcp://*:5555')
object_data_path = r"C:/Users/MSI-NB/Final project/data/"
list_file = os.listdir(object_data_path)
print(list_file[0].split(".")[0])
while True:
    start = time.time()
    print("server starts")

    object_to_match = ""
    Is_Object = False
    Is_Qust = ""
    match_index = 0
    all_objects = []
    back_data = {}
    
    data_rec = socket.recv()
    data_rec = data_rec.decode()
    sentence = Sentence(data_rec)
    tagger.predict(sentence)
    for entity in sentence.get_spans('np'):
        
        if ("NP" == entity.get_label().value or "ADVP" == entity.get_label().value) and Is_Qust=="":
            print(entity.get_label().value)
            if entity.tokens[0].form == "where":
                Is_Qust = "position"
            elif entity.tokens[0].form == "what" or entity.tokens[0].form == "which":
                Is_Qust = ""
                for token in entity.tokens[1:]:
                    Is_Qust = Is_Qust + token.form
        if "VP" == entity.get_label().value and len(Is_Qust)>0:
            Is_Object = True
        if "NP" == entity.get_label().value and Is_Object==True:
            for token in entity.tokens:
                object_to_match = object_to_match + token.form + " "
    if Is_Object:
        list_file = os.listdir(object_data_path)
        object_type = list_file[0].split(".")[0]
        file_path = object_data_path + list_file[0]
        file = open(file_path)
        object_datas = json.load(file)
        for object in object_datas:
            object_string = object_type + " "
            for key in object.keys():
                if key != "position" and key != "name":
                    object_string = object_string + object[key] + " "
            all_objects.append(object_string)
    if Is_Qust != "":
        len_all = len(all_objects)
        all_objects2match = all_objects.copy()
        all_objects2match.append(object_to_match)
        all_objects_embeddings = model_segmentation.encode(all_objects2match)
        scores = cosine_similarity([all_objects_embeddings[len_all]], all_objects_embeddings[:len_all])
        for score in scores[0]:
            if score > 0.9:
                break
            match_index = match_index + 1
        if match_index == len(scores[0]):
            Is_Object =  False

    if Is_Object:
        response = object_datas[match_index][Is_Qust]
    else:
        Is_Qust = "regular"
        new_user_input_ids = tokenizer.encode(data_rec + tokenizer.eos_token, return_tensors='pt')

        # append the new user input tokens to the chat history
        bot_input_ids = torch.cat([chat_history_ids, new_user_input_ids], dim=-1) if step > 0 else new_user_input_ids

        # generated a response while limiting the total chat history to 1000 tokens, 
        chat_history_ids = model_regular.generate(bot_input_ids, max_length=1000, pad_token_id=tokenizer.eos_token_id)

        # pretty print last ouput tokens from bot
        response = "Bot: {}".format(tokenizer.decode(chat_history_ids[:, bot_input_ids.shape[-1]:][0], skip_special_tokens=True))

        step = step + 1

    back_data["response"] = response
    back_data["response_cata"] = Is_Qust
    socket.send_json(back_data)
    end = time.time()
    print('FPS:', 1 / (end - start))