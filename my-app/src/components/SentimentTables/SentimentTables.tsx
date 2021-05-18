import React, { useEffect, useState } from "react";
import LocationTable from '../LocationTable/LocationTable';
import { ResponseList } from '../Interface/ResponseList';

const SentimentTables = () => {

    const data: ResponseList = {
        responseList: []
    };

    const [apiReponse, setApiReponseList] = useState<ResponseList>(data);

    const getSentimentData = () => {
        console.log("about to fetch data");
        debugger;
        fetch('https://localhost:44326/twitter/average')
        .then(resp => resp.json())
        .then(data => {
            console.log("about to set data");
            setApiReponseList({...apiReponse, responseList: data });
            console.log("data", apiReponse);
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


