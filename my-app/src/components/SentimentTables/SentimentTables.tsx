import React, { useEffect, useState } from "react";
import LocationTable from '../LocationTable/LocationTable';
import { ResponseList } from '../Interface/ResponseList';

const SentimentTables = () => {

    const data: ResponseList = {
        responseList: []
    };

    const [apiReponse, setApiReponseList] = useState<ResponseList>(data);

    const getSentimentData = () => {
        fetch('https://localhost:44326/twitter/average')
        .then(resp => resp.json())
        .then(data => {
            setApiReponseList({...apiReponse, responseList: data });
        });                
    }    

    useEffect(() => {
        getSentimentData();
    }, []);

    return(    
        <>    
        {apiReponse && apiReponse?.responseList?.length > 0 &&
        apiReponse?.responseList?.map((row, index) => (
        <LocationTable {...row}/> ))        
        }
        </>
    );
}

export default SentimentTables;


