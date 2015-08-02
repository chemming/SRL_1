
cd('haldata')
fileList = dir('playDat*');

beforeXY = [];
afterXY = [];
beforeOrient = [];
afterOrient = [];
itemXY = [];
goalXY = [];

for f = 1:size(fileList,1)
    fileName = fileList(f).name;
    load(fileName);
    for d = 1:size(simDat,1)
        dataRow = simDat(d);
        dataRow.beforePos;
        dataRow.afterPos;
        beforeXY = [beforeXY;  dataRow.beforePos(1) dataRow.beforePos(3)];
        afterXY = [afterXY;  dataRow.afterPos(1) dataRow.afterPos(3)];
        goalXY = [goalXY; dataRow.goal(1) dataRow.goal(3)];
        itemXY = [itemXY; dataRow.item(1) dataRow.item(3)];
        beforeOrient = [beforeOrient ;  dataRow.beforeOrient];
        afterOrient = [afterOrient ;  dataRow.afterOrient];
    end
end
cd('..')

displacementXY = afterXY - beforeXY; 
directionXY = bsxfun(@rdivide,vectorXY,sqrt(sum(vectorXY.^2,2)));
itemXY = itemXY - beforeXY;
goalXY = goalXY - beforeXY;
angXY = afterOrient - beforeOrient;

hold on
%scatter(displacementXY (:,1),displacementXY (:,2) ,'x');
%scatter(itemXY (:,1),itemXY (:,2) ,'o');
%scatter(goalXY (:,1),goalXY (:,2) ,'.');


dataFinal = [itemXY goalXY angXY];

