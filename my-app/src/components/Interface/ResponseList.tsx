export interface ResponseList {
    responseList: Response[];
}

export interface Response {
    tweets: string[];
	location: string;
	averageSentimentValue: number;
}
