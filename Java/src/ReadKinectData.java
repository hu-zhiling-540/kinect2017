import org.json.simple.JSONArray;
import org.json.simple.JSONObject;

/**
 * Created by Zhiling on 5/30/17.
 */
public class ReadKinectData {

    private void readJsonStrData(String dataList ) {

        Object obj = dataList;
        JSONObject jsonObj = (JSONObject) obj;

        JSONArray bodies = (JSONArray) jsonObj.get("bodies");

        for(int i = 0;  i < bodies.size(); i++) {
            JSONObject body = (JSONObject) bodies.get(i);
            if(body.get("tracked").equals("true"))  {
                JSONArray joints = (JSONArray) body.get("joints");
                for (int j = 0; j < joints.size(); j++) {
                    JSONObject joint = (JSONObject) joints.get(j);
                    System.out.println(joint.get("depthX"));
                }

            }
        }
    }
}
