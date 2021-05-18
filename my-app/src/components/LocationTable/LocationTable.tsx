import Accordion from '@material-ui/core/Accordion';
import { Table, TableBody, TableCell, TableContainer, TableRow, Typography } from '@material-ui/core';
import AccordionDetails from '@material-ui/core/AccordionDetails';
import React from "react";
import { useStyles, ExpansionPanelSummary } from "../ExpansionPanel/ExpansionPanel";
import { Response } from '../Interface/ResponseList';

const LocationTable = (response: Response) => {
    console.log("response is ", response);  
    const [expanded, setExpanded] = React.useState<string | false>('panel1');
    const handleChange = (panel: string) => (event: React.ChangeEvent<{}>, isExpanded: boolean) => {
        setExpanded(isExpanded ? panel : false);
    };
    const classes = useStyles();
    return(
        <div>
            <Accordion key={'LocationPanel'} expanded={expanded === 'panel1'} onChange={handleChange('panel1')} elevation={0}>
                <ExpansionPanelSummary expandIcon={<img src="https://ww2-secure.justanswer.com/static/fe/ja-icons/svg/arrow-up.svg" />}>
                    <Typography>{response.location}</Typography>
                    <Typography>{response.averageSentimentValue}</Typography>
                </ExpansionPanelSummary>
            
                <AccordionDetails>
                    <TableContainer>
                        <Table stickyHeader aria-label="sticky table">
                            <TableBody>
                                {response && response?.tweets?.length > 0 &&
                                response?.tweets?.map((row, index) => (
                                    <TableRow>
                                        <TableCell>
                                            {row}
                                        </TableCell>
                                    </TableRow>
                                ))
                                }
                            </TableBody>
                        </Table>
                    </TableContainer>
                </AccordionDetails>
            </Accordion>
        </div>
    );
}

export default LocationTable;